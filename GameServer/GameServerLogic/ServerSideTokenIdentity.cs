using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public enum UserMatchmakingStatus
    {
        QUEUE,
        GAME,
        PREPARING_GAME,
        LOBBY,
        NON_LOGGED
    }

    public class ServerSideTokenIdentity
    {
        public User LastlyFetchedUser { get; set; }
        public AsyncUserToken Token { get; set; }

        public UserMatchmakingStatus MatchmakingStatus { get; set; }
        public object MatchmakingLock { get; set; } = new object();

        public ServerSideTokenIdentity()
        {
            Reset();
        }

        public void Reset()
        {
            LastlyFetchedUser = null;
            Token = null;
            MatchmakingStatus = UserMatchmakingStatus.NON_LOGGED;
        }
    }
}
