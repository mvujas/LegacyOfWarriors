using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;
using Utils.Interface;
using Utils.Net;
using System.Net;
using System.Net.Sockets;
using Utils.Remote;
using Utils.Delegates;

namespace ClientUtils
{
    public class SocketClient
    {
        private AsyncUserToken m_userToken;
        private IMessageHandlingContainer m_eventHandlingContainer;

        public Runnable OnDisconnect { get; set; }

        public SocketClient(IMessageHandlingContainer eventHandlingContainer, 
            int receiveBufferSize = 10)
        {
            if (receiveBufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("Buffer size must be positive number");
            }
      

            m_eventHandlingContainer = eventHandlingContainer 
                ?? throw new ArgumentNullException(nameof(eventHandlingContainer));

            InitUserToken(receiveBufferSize);
        }

        private void InitUserToken(int bufferSize)
        {
            var readEventArgs = new SocketAsyncEventArgs();
            var writeEventArgs = new SocketAsyncEventArgs();

            readEventArgs.Completed += IO_Completed;
            writeEventArgs.Completed += IO_Completed;

            byte[] buffer = new byte[bufferSize];
            readEventArgs.SetBuffer(buffer, 0, buffer.Length);

            m_userToken = new AsyncUserToken(readEventArgs, writeEventArgs);
            m_userToken.OnMessageError = m_eventHandlingContainer.OnMessageError;
            m_userToken.OnMessageFullyReceived = m_eventHandlingContainer.OnMessageReceived;
        }

        public void Send(byte[] message, bool sendPlain = false)
        {
            if (!IsActive())
            {
                throw new InvalidOperationException("Socket client must run to send messge");
            }

            if (!sendPlain)
            {
                message = MessageTransformer.PrepareMessageForSending(message);
            }
            m_userToken.Send(message);
        }

        public void Receive()
        {
            bool asyncExecution = m_userToken.Socket.ReceiveAsync(m_userToken.ReadEventArgs);
            if (!asyncExecution)
            {
                ProcessSend(m_userToken.ReadEventArgs);
            }
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

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Disconnect();
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                token.Process(e.Buffer, e.BytesTransferred);
                Receive();
            }
            else
            {
                Disconnect();
            }
        }

        public bool IsActive()
        {
            return m_userToken.Socket != null;
        }

        /// <summary>
        /// Throws exception is something is wrong
        /// </summary>
        public void Connect(IPEndPoint endPoint)
        {
            if (IsActive())
            {
                throw new InvalidOperationException("Socket client is already active");
            }

            try
            {
                m_userToken.Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_userToken.Socket.Connect(endPoint);
                Receive();
            }
            catch(Exception)
            {
                m_userToken.Socket = null;
                throw;
            }
        }

        public void Disconnect()
        {
            if (!IsActive())
            {
                return;
            }

            try
            {
                m_userToken.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            m_userToken.Socket.Close();

            m_userToken.Socket = null;

            OnDisconnect?.Invoke();
        }
    }
}
