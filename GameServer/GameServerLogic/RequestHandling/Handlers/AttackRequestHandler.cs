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
    public class AttackRequestHandler : RequestHandler
    {
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            AttackRequest cardPlayRequest = request as AttackRequest;
            AttackResponse response = new AttackResponse
            {
                Successfulness = true,
                Message = null
            };
            try
            {
                GAME_MANAGER.Attack(token, cardPlayRequest.AttackingUnit, cardPlayRequest.TargetPlayer, cardPlayRequest.TargetUnit);
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
