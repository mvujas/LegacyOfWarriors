using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Implementation;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class EndTurnRequestHandler : RequestHandler
    {
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            EndTurnResponse response = new EndTurnResponse
            {
                Successfulness = true,
                Message = null
            };
            try
            {
                GAME_MANAGER.EndTurn(token);
            }
            catch(MatchmakingException ex)
            {
                response.Successfulness = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
