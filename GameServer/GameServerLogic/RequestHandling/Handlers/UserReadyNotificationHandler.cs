using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class UserReadyNotificationHandler : RequestHandler
    {
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            try
            {
                GAME_MANAGER.MarkUserAsReadyForGame(token);
            }
            catch(MatchmakingException ex)
            {
                Console.WriteLine("Matchmaking exception while marking as ready: " + ex.Message);
            }
            return null;
        }
    }
}
