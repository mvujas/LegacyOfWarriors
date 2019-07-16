using System;
using System.Net.Sockets;
using Utils;

namespace GameServer.Net
{
    public class AsyncUserToken
    {
        #region EVENTS
        public event Action<MessageWrapper> OnMessageFullyReceived;
        public event Action<AsyncUserToken> OnMessageError;
        #endregion

        public Socket Socket { get; set; }

        private SocketAsyncEventArgs m_readEventArgs;
        public SocketAsyncEventArgs ReadEventArgs
        {
            get => m_readEventArgs;
            set
            {
                m_readEventArgs = value;
                if (m_readEventArgs != null)
                {
                    m_readEventArgs.UserToken = this;
                }
            }
        }

        private SocketAsyncEventArgs m_writeEventArgs;
        public SocketAsyncEventArgs WriteEventArgs
        {
            get => m_writeEventArgs;
            set
            {
                m_writeEventArgs = value;
                if (m_writeEventArgs != null)
                {
                    m_writeEventArgs.UserToken = this;
                }
            }
        }

        private int m_bytesToReceive;
        private byte[] m_messageBuffer = null;

        private object m_receivingMessageLock = new object();

        public void PassMessage(byte[] message)
        {
            lock(m_receivingMessageLock)
            {
                try
                {
                    if (m_messageBuffer == null)
                    {
                        byte[] lengthBuffer = ArrayUtils.SubArray(message, 0, 4);
                        int length = BitConverter.ToInt32(lengthBuffer, 0);
                        m_messageBuffer = new byte[length];
                        Array.Copy(message, 4, m_messageBuffer, 0, message.Length - 4);
                    }
                    else
                    {

                    }
                }
                catch(Exception)
                {
                    OnMessageError(this);
                }
            }
        }
    }
}
