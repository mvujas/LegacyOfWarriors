using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.GameServerLogic
{
    public static class CardListManager
    {
        private static CardList cardList = new CardList("v1.0");
        static CardListManager()
        {
            AddCardsToCardList();
        }
        public static CardList GetCardList()
        {
            return cardList;
        }

        private static void AddCardsToCardList()
        {
            cardList.AddCard(name: "Druid", clientSideImage: "druid", 
                cost: 1, attack: 5, health: 10);
        }
    }
}
