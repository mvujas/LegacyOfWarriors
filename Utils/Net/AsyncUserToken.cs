using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Utils.Net
{
    public class AsyncUserToken
    {
        public Socket Socket { get; set; }
        private SocketAsyncEventArgs m_readEventArgs;
        private SocketAsyncEventArgs m_writeEventArgs;
        private MessageReceiver m_messageReceiver;

        public AsyncUserToken()
        {
            m_messageReceiver = new MessageReceiver(this);
        }

        #region PROPERTIES
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
        public Action<AsyncUserToken> OnMessageError
        {
            set => m_messageReceiver.OnMessageError = value;
        }
        public Action<MessageWrapper> OnMessageFullyReceived
        {
            set => m_messageReceiver.OnMessageFullyReceived = value;
        }
        #endregion

        public void Process(byte[] message)
        {
            m_messageReceiver.Process(message);
        }

    }
}
