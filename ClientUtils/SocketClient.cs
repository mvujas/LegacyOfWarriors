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

namespace ClientUtils
{
    public class SocketClient
    {
        private AsyncUserToken m_userToken;
        private IConnectionEventHandler m_connectionHandler;

        public SocketClient(IConnectionEventHandler connectionHandler, 
            int receiveBufferSize = 10)
        {
            if(receiveBufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("Buffer size must be positive number");
            }

            m_connectionHandler = connectionHandler ?? 
                throw new ArgumentNullException(nameof(connectionHandler));

            InitUserToken(receiveBufferSize);
        }

        private void InitUserToken(int bufferSize)
        {
            var readEventArgs = new SocketAsyncEventArgs();
            var writeEventArgs = new SocketAsyncEventArgs();

            byte[] buffer = new byte[bufferSize];
            readEventArgs.SetBuffer(buffer, 0, buffer.Length);

            m_userToken = new AsyncUserToken(readEventArgs, writeEventArgs);
        }

        private bool IsActive()
        {
            return m_userToken.Socket != null;
        }

        public void Connect(IPEndPoint endPoint)
        {
            if (IsActive())
            {
                throw new InvalidOperationException("Socket client is already active");
            }

            m_userToken.Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                m_userToken.Socket.Connect(endPoint);
                m_connectionHandler.OnSuccessfulConnection();
            }
            catch(Exception)
            {
                m_connectionHandler.OnFailedConnection();
            }
        }

        public void Disconnect()
        {
            try
            {
                m_userToken.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            m_userToken.Socket.Close();

            m_userToken.Socket = null;

            m_connectionHandler.OnDisconnect();
        }

        public void Send(byte[] message)
        {
            if(!IsActive())
            {
                throw new InvalidOperationException("Socket client must run to send messge");
            }
        }
    }
}
