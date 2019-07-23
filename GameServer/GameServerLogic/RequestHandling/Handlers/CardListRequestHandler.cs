using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Implementation;
using Remote.InGameObjects;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;
using Utils;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class CardListRequestHandler : RequestHandler
    {
        private CardListResponse m_newListResponse;
        private CardList m_cardList;
        private CardListResponse m_upToDateResponse = new CardListResponse { UpToDate = true };
        public CardListRequestHandler()
        {
            PrepareSuccessfulAnswer();
        }

        private void PrepareSuccessfulAnswer()
        {
            m_cardList = CardListManager.GetCardList();

            m_newListResponse = new CardListResponse
            {
                UpToDate = false,
                CardList = m_cardList
            };
        }

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            CardListRequest cardListRequest = request as CardListRequest;
            if(cardListRequest.Version == null || cardListRequest.Version != m_cardList.Vesion)
            {
                return m_newListResponse;
            }
            return m_upToDateResponse;
        }
    }
}
