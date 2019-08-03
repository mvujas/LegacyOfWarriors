using GameServer.GameServerLogic.Lists;
using GameServer.GameServerLogic.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Net;
using Utils.Remote;
using Utils.Threading;

namespace GameServer.GameServerLogic
{
    public class CardGameLogicRouter : ServerSideLogicRouter
    {
        private RemoteRequestMapper m_requestMapper = new CardGameRequestMapper();
        private Matchmaker m_matchmaker = Matchmaker.GetInstance();

        public override void OnUserConnect(AsyncUserToken userToken)
        {
            ServerSideTokenIdentity identity = (ServerSideTokenIdentity)userToken.info;
            if (userToken.info == null)
            {
                identity = new ServerSideTokenIdentity();
                userToken.info = identity;
            }
            identity.Token = userToken;
            identity.MatchmakingStatus = UserMatchmakingStatus.NON_LOGGED;
            Console.WriteLine("User connected");
        }

        public override void OnUserDisconnect(AsyncUserToken userToken)
        {
            var identity = (ServerSideTokenIdentity)userToken.info;

            ThreadUtils.RepeatingTimeoutLock(
                identity.MatchmakingLock,
                () => ResolveMatchmakingStatusOnQuit(userToken),
                millisecondsTimeOut: 1,
                maxAttempts: 0,
                interAttemptyDelay: 5
            );

            UserConnectionList.GetInstance().LogOutUserUnderIdentity(identity);
            identity.Reset();
            Console.WriteLine("User disconnected");
        }

        private void ResolveMatchmakingStatusOnQuit(AsyncUserToken userToken)
        {
            var identity = (ServerSideTokenIdentity)userToken.info;
            switch (identity.MatchmakingStatus)
            {
                case UserMatchmakingStatus.QUEUE:
                {
                    try
                    {
                        m_matchmaker.ExitQueue(userToken);
                        Console.WriteLine("Napustio red pri izlasku!");
                    }
                    catch (Exception) { }
                    break;
                }
                case UserMatchmakingStatus.GAME:

                    break;
            }
        }

        protected override RemoteRequestMapper GetRequestMapper()
        {
            return m_requestMapper;
        }
    }
}
