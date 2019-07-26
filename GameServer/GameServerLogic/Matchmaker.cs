using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public class UserQueueWrapper
    {
        public AsyncUserToken Token { get; set; }
        public List<Card> deck { get; set; }
    }

    public class Matchmaker
    {
        private UserQueueWrapper userWaitingForMatch = null;
        private object matchingLock = new object();
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public void AddUserToQueue(UserQueueWrapper queueWrapper)
        {
            AsyncUserToken userToken = queueWrapper.Token;
            ServerSideTokenIdentity identity = userToken.info as ServerSideTokenIdentity;
            lock(identity.MatchmakingLock)
            {
                if(identity.MatchmakingStatus != UserMatchmakingStatus.NONE)
                {
                    throw new MatchmakingException("Korisnik je vec u redu ili igri");
                }
            }
        }

        private void AddOrMatch(UserQueueWrapper queueWrapper)
        {
            UserQueueWrapper otherPlayer = null;
            lock (matchingLock)
            {
                AsyncUserToken userToken = queueWrapper.Token;
                if (userWaitingForMatch == null)
                {
                    userWaitingForMatch = queueWrapper;
                    ((ServerSideTokenIdentity)userToken.info).MatchmakingStatus = UserMatchmakingStatus.QUEUE;
                }
                else
                {
                    otherPlayer = userWaitingForMatch;
                    userWaitingForMatch = null;
                }
            }
            if(otherPlayer != null)
            {
                GAME_MANAGER.CreateGame(queueWrapper, otherPlayer);
            }
        }
    }
}
