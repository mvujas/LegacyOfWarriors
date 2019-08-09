using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Implementation;
using Remote.Interface;
using Utils.GameLogicUtils;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class PlayCardRequestHandler : RequestHandler
    {
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            PlayCardRequest playCardRequest = request as PlayCardRequest;
            PlayCardResponse response = new PlayCardResponse
            {
                Successfulness = true,
                Message = null
            };
            try
            {
                GAME_MANAGER.PlayCard(token, playCardRequest.CardInGameId);
            }
            catch (LogicExecutionException e)
            {
                response.Successfulness = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}
