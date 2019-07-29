using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Implementation;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;
using Utils;
using Remote.InGameObjects;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class QueueEntryRequestHandler : RequestHandler
    {
        private Matchmaker matchmaker = Matchmaker.GetInstance();
        private CardList cardList = CardListManager.GetCardList();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            QueueEntryRequest queueEntryRequest = request as QueueEntryRequest;

            var response = new QueueEntryResponse();
            try
            {
                var deck = CardUtils.CardIdsToLinkedList(cardList, queueEntryRequest.Deck);
                var queueEntry = new UserQueueWrapper
                {
                    Token = token,
                    Deck = deck
                };

                matchmaker.AddUserToQueue(queueEntry);

                response.Successfulness = true;
            }
            catch(Exception e)
            {
                if(e is CardUtilsException || e is MatchmakingException)
                {

                    response.Successfulness = false;
                    response.Message = e.Message;
                }
                else
                {
                    throw;
                }
            }

            return response;
        }
    }
}
