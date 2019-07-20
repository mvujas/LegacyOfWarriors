using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Utils.Net
{
    public class AsyncUserToken
    {
        private static int id = int.MinValue;

        private int currentId;

        private Queue<byte[]> m_messageSendingQueue = new Queue<byte[]>();

        public Socket Socket { get; set; }
        public object info { get; set; }
        private SocketAsyncEventArgs m_readEventArgs;
        private SocketAsyncEventArgs m_writeEventArgs;
        private MessageReceiver m_messageReceiver;
        

        public AsyncUserToken(SocketAsyncEventArgs readEventArgs, SocketAsyncEventArgs writeEventArgs)
        {
            m_messageReceiver = new MessageReceiver(this);
            currentId = Interlocked.Increment(ref id);
            ReadEventArgs = readEventArgs;
            WriteEventArgs = writeEventArgs;

            WriteEventArgs.Completed += (sender, args) => RemoveFirstAndTryToSendNext();
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

        #region MESSAGE RECEIVING
        public void Process(byte[] message, int messageBytes)
        {
            m_messageReceiver.Process(message, messageBytes);
        }
        #endregion

        private bool IsSocketActiveAndResetSendingQueue()
        {
            bool isActive = Socket != null;

            if (!isActive)
            {
                m_messageSendingQueue.Clear();
            }

            return isActive;
        }

        #region MESSAGE SENDING
        public void Send(byte[] message)
        {
            if (IsSocketActiveAndResetSendingQueue())
            {
                lock (m_messageSendingQueue)
                {
                    bool isEmpty = m_messageSendingQueue.Count == 0;
                    m_messageSendingQueue.Enqueue(message);
                    if (isEmpty)
                    {
                        ActuallySendMessage(message);
                    }
                }
            }
        }

        private void RemoveFirstAndTryToSendNext()
        {
            if (IsSocketActiveAndResetSendingQueue())
            {
                lock (m_messageSendingQueue)
                {
                    m_messageSendingQueue.Dequeue();
                    try
                    {
                        byte[] message = m_messageSendingQueue.Peek();
                        ActuallySendMessage(message);
                    }
                    catch (Exception) { }
                }
            }
        }

        private void ActuallySendMessage(byte[] message)
        {
            if (IsSocketActiveAndResetSendingQueue())
            {
                WriteEventArgs.SetBuffer(message, 0, message.Length);
                Socket.SendAsync(WriteEventArgs);
            }
        }
        #endregion

        #region OVERRIDEN METHODS
        public override bool Equals(object obj)
        {
            return obj is AsyncUserToken token &&
                   currentId == token.currentId;
        }

        public override int GetHashCode()
        {
            return -1353522673 + currentId.GetHashCode();
        }
        #endregion
    }
}
