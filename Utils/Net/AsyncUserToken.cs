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

        public override bool Equals(object obj)
        {
            return obj is AsyncUserToken token &&
                   EqualityComparer<Socket>.Default.Equals(Socket, token.Socket) &&
                   EqualityComparer<SocketAsyncEventArgs>.Default.Equals(m_readEventArgs, token.m_readEventArgs) &&
                   EqualityComparer<SocketAsyncEventArgs>.Default.Equals(m_writeEventArgs, token.m_writeEventArgs) &&
                   EqualityComparer<MessageReceiver>.Default.Equals(m_messageReceiver, token.m_messageReceiver) &&
                   EqualityComparer<SocketAsyncEventArgs>.Default.Equals(ReadEventArgs, token.ReadEventArgs) &&
                   EqualityComparer<SocketAsyncEventArgs>.Default.Equals(WriteEventArgs, token.WriteEventArgs);
        }

        public override int GetHashCode()
        {
            var hashCode = 1998376283;
            hashCode = hashCode * -1521134295 + EqualityComparer<Socket>.Default.GetHashCode(Socket);
            hashCode = hashCode * -1521134295 + EqualityComparer<SocketAsyncEventArgs>.Default.GetHashCode(m_readEventArgs);
            hashCode = hashCode * -1521134295 + EqualityComparer<SocketAsyncEventArgs>.Default.GetHashCode(m_writeEventArgs);
            hashCode = hashCode * -1521134295 + EqualityComparer<MessageReceiver>.Default.GetHashCode(m_messageReceiver);
            hashCode = hashCode * -1521134295 + EqualityComparer<SocketAsyncEventArgs>.Default.GetHashCode(ReadEventArgs);
            hashCode = hashCode * -1521134295 + EqualityComparer<SocketAsyncEventArgs>.Default.GetHashCode(WriteEventArgs);
            return hashCode;
        }
    }
}
