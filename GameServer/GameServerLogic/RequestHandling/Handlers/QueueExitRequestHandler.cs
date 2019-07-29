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
    public class QueueExitRequestHandler : RequestHandler
    {
        private Matchmaker matchmaker = Matchmaker.GetInstance();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            QueueExitRequest queueExitRequest = request as QueueExitRequest;

            QueueExitResponse response = new QueueExitResponse();
            try
            {
                matchmaker.ExitQueue(token);

                response.Successfulness = true;
            }
            catch(MatchmakingException e)
            {
                response.Successfulness = false;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
