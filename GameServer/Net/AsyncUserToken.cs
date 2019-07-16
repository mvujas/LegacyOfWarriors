using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Net
{
    public class AsyncUserToken
    {
        public Socket Socket { get; set; }
        public SocketServer Server { get; set; }

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


    }
}
