using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace Utils.Net
{
    internal class MessageReceiver
    {
        public MessageReceiver(AsyncUserToken userToken)
        {
            m_userToken = userToken ?? 
                throw new ArgumentNullException(nameof(userToken));
        }

        public Action<AsyncUserToken> OnMessageError { set; private get; }
        public Action<MessageWrapper> OnMessageFullyReceived { set; private get; }

        private AsyncUserToken m_userToken;

        private int m_bytesToReceive = 0;
        private byte[] m_messageBuffer = null;

        private object m_receivingMessageLock = new object();

        public void Process(byte[] message)
        {
            lock(m_receivingMessageLock)
            {
                try
                {
                    ContinueReceive(message);
                }
                catch(Exception)
                {
                    ResetAndReturnError();
                }
            }
        }

        private void ContinueReceive(byte[] message)
        {
            int start, bytesToGet;
            if(m_messageBuffer == null)
            {
                int length = BitConverter.ToInt32(message, 0);
                m_bytesToReceive = length;
                m_messageBuffer = new byte[length];
                start = 4;
                bytesToGet = Math.Min(message.Length - 4, length);
            }
            else
            {
                start = 0;
                bytesToGet = Math.Min(message.Length, m_bytesToReceive);
            }
            int firstEmptyIndex = m_messageBuffer.Length - m_bytesToReceive;
            Array.Copy(message, start, m_messageBuffer, firstEmptyIndex, bytesToGet);
            m_bytesToReceive -= bytesToGet;
            Console.WriteLine("Left to receive: " + m_bytesToReceive);
            if (m_bytesToReceive == 0)
            {
                ProcessFullyReceivedMessage();
            }
        }

        private void Reset()
        {
            m_bytesToReceive = 0;
            m_messageBuffer = null;
        }

        private void ResetAndReturnError()
        {
            Reset();
            OnMessageError(m_userToken);
        }

        private void ProcessFullyReceivedMessage()
        {
            MessageWrapper messageWrapper = new MessageWrapper
            {
                Message = m_messageBuffer, 
                UserToken = m_userToken
            };
            Reset();
            OnMessageFullyReceived(messageWrapper);
        }
    }
}
