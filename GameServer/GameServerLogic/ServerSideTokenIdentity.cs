using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public class ServerSideTokenIdentity
    {
        public User LastlyFetchedUser { get; set; }
        public AsyncUserToken Token { get; set; }

        public ServerSideTokenIdentity()
        {
            Reset();
        }

        public void Reset()
        {
            LastlyFetchedUser = null;
            Token = null;
        }
    }
}
