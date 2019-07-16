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

namespace GameServer.Net
{
    public class SocketServer
    {
        private int m_numConnections;
        private int m_currentlyConnectedUsers;
        private int m_bufferSize;
        private Socket m_listener;
        private ObjectPool<SocketAsyncEventArgs> m_readSocketPool;
        private ObjectPool<SocketAsyncEventArgs> m_writeSocketPool;
        private ObjectPool<AsyncUserToken> m_userTokens;
        private AsyncSocketArgsBufferManager m_bufferManager;
        private Semaphore m_maxClientsAccepted;
        private int m_backLog;

        public bool IsRunning { get; private set; }

        public EventHandlingContainer m_eventHandlers;


        public SocketServer(int numConnections, int bufferSize, 
            EventHandlingContainer eventHandlers, int backLog = 10)
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
            m_bufferManager = new AsyncSocketArgsBufferManager(m_bufferSize * m_numConnections, m_bufferSize);

            m_maxClientsAccepted = new Semaphore(m_numConnections, m_numConnections);
            Init();
        }

        private void Init()
        {
            m_bufferManager.InitBuffer();

            Supplier<SocketAsyncEventArgs> writeSocketSupplier = () =>
            {
                SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
                socketEventArgs.Completed += IO_Completed;

                return socketEventArgs;
            };

            Supplier<SocketAsyncEventArgs> readSocketSupplier = () => {
                SocketAsyncEventArgs socketEventArgs = writeSocketSupplier();

                m_bufferManager.SetBuffer(socketEventArgs);
                return socketEventArgs;
            };
            m_readSocketPool = new ObjectPool<SocketAsyncEventArgs>(readSocketSupplier,
                m_numConnections);
            m_writeSocketPool = new ObjectPool<SocketAsyncEventArgs>(writeSocketSupplier,
                m_numConnections);

            Supplier<AsyncUserToken> userTokenSupplier = () =>
            {
                AsyncUserToken userToken = new AsyncUserToken();
                userToken.ReadEventArgs = m_readSocketPool.GetObject();
                userToken.WriteEventArgs = m_writeSocketPool.GetObject();
                userToken.OnMessageFullyReceived = m_eventHandlers.OnMessageReceived;
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

            //OnConnect.BeginInvoke(this, userToken, null, null);

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
                token.Process(e.Buffer);
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
            SocketAsyncEventArgs writeEventArgs = token.WriteEventArgs;

            byte[] dataToSend = MessageTransformer.PrepareMessageForSending(message);

            writeEventArgs.SetBuffer(dataToSend, 0, dataToSend.Length);

            bool asyncExecution = token.Socket.SendAsync(writeEventArgs);
            if (!asyncExecution)
            {
                ProcessSend(writeEventArgs);
            }
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

            /*OnDisconnect.BeginInvoke(this, token, result => {
                Interlocked.Decrement(ref m_currentlyConnectedUsers);

                m_userTokens.ReleaseObject(token);
                m_readSocketPool.ReleaseObject(token.ReadEventArgs);
                m_writeSocketPool.ReleaseObject(token.WriteEventArgs);

                m_maxClientsAccepted.Release();
            }, null);*/
        }



    }
}
