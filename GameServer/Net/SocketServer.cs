using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Utils.DataTypes;
using Utils;
using Utils.Delegates;
using Utils.Net;
using Utils.Interface;
using System.ServiceModel.Channels;

namespace GameServer.Net
{
    public class SocketServer
    {
        private int m_numConnections;
        private int m_currentlyConnectedUsers;
        private int m_bufferSize;
        private Socket m_listener;
        private ObjectPool<AsyncUserToken> m_userTokens;
        private Semaphore m_maxClientsAccepted;
        private int m_backLog;

        public bool IsRunning { get; private set; }

        public IEventHandlingContainer m_eventHandlers;

        public SocketServer(int numConnections, int bufferSize, 
            IEventHandlingContainer eventHandlers, int backLog = 10)
        {
            if (numConnections < 1)
            {
                throw new ArgumentException("Number of connections must be positive number");
            }
            if (bufferSize < 1)
            {
                throw new ArgumentException("Buffer size must be positive number");
            }
            if (backLog < 1)
            {
                throw new ArgumentException("BackLog must be positive number");
            }
            m_eventHandlers = eventHandlers ?? 
                throw new ArgumentNullException(nameof(eventHandlers));

            IsRunning = false;
            m_currentlyConnectedUsers = 0;
            m_numConnections = numConnections;
            m_bufferSize = bufferSize;
            m_backLog = backLog;

            m_maxClientsAccepted = new Semaphore(m_numConnections, m_numConnections);
            Init();
        }

        private void Init()
        {
            Supplier<SocketAsyncEventArgs> writeSocketSupplier = () =>
            {
                SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
                socketEventArgs.Completed += IO_Completed;

                return socketEventArgs;
            };

            Supplier<SocketAsyncEventArgs> readSocketSupplier = () => {
                SocketAsyncEventArgs socketEventArgs = writeSocketSupplier();
                byte[] buffer = new byte[m_bufferSize];

                socketEventArgs.SetBuffer(buffer, 0, buffer.Length);

                return socketEventArgs;
            };

            Supplier<AsyncUserToken> userTokenSupplier = () =>
            {
                AsyncUserToken userToken = 
                    new AsyncUserToken(readSocketSupplier(), writeSocketSupplier());
                userToken.OnMessageFullyReceived = m_eventHandlers.OnMessageReceived;
                userToken.OnMessageError = m_eventHandlers.OnMessageError;
                return userToken;
            };
            m_userTokens = new ObjectPool<AsyncUserToken>(userTokenSupplier, m_numConnections);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(IPEndPoint localEndPoint)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running");
            }
            IsRunning = true;
            m_listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_listener.Bind(localEndPoint);
            m_listener.Listen(m_backLog);

            StartAccept();

            Console.WriteLine("Socket server is running on {0}", localEndPoint.ToString());
            Console.WriteLine("Press any key to terminate the server process....");
            Console.ReadKey();
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArg = null)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += AcceptEventArg_Completed;
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }

            m_maxClientsAccepted.WaitOne();
            bool processAsynchronously = m_listener.AcceptAsync(acceptEventArg);
            if (!processAsynchronously)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_currentlyConnectedUsers);

            AsyncUserToken userToken = m_userTokens.GetObject();
            userToken.Socket = e.AcceptSocket;

            m_eventHandlers.OnUserConnect(userToken);

            Receive(userToken);

            StartAccept(e);
        }

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException(
                        "The last operation completed on the socket was not a receive or send");
            }
        }

        public void Receive(AsyncUserToken token)
        {
            bool asyncExecution = token.Socket.ReceiveAsync(token.ReadEventArgs);
            if (!asyncExecution)
            {
                ProcessSend(token.ReadEventArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                token.Process(e.Buffer, e.BytesTransferred);
                Receive(token);
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        public void Send(AsyncUserToken token, string message)
        {
            Send(token, Encoding.ASCII.GetBytes(message));
        }

        public void Send(AsyncUserToken token, byte[] message)
        {
            token.Send(message);
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                CloseClientSocket(e);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;

            if (token.Socket == null)
            {
                return;
            }

            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            token.Socket.Close();
            token.Socket = null;

            m_eventHandlers.OnUserDisconnect(token);

            Interlocked.Decrement(ref m_currentlyConnectedUsers);

            m_userTokens.ReleaseObject(token);

            m_maxClientsAccepted.Release();
        }
    }
}
