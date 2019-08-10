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
        private static readonly CardList cardList = new CardList("v1.0.0");
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
            cardList.AddCard(name: "Cvet", clientSideImage: "flower", 
                cost: 0, attack: 1, health: 1);
            cardList.AddCard(name: "Zemljoradnik", clientSideImage: "villager",
                cost: 1, attack: 3, health: 2);
            cardList.AddCard(name: "Ovan", clientSideImage: "ram",
                cost: 2, attack: 7, health: 1);
            cardList.AddCard(name: "Strelac", clientSideImage: "archer",
                cost: 4, attack: 6, health: 4);
            cardList.AddCard(name: "Čarobnjak", clientSideImage: "wizard",
                cost: 4, attack: 7, health: 3);
            cardList.AddCard(name: "Čuvar", clientSideImage: "protector",
                cost: 6, attack: 2, health: 20);
            cardList.AddCard(name: "Ratnik", clientSideImage: "warrior",
                cost: 6, attack: 7, health: 7);
            cardList.AddCard(name: "Legendarni zamak", clientSideImage: "fortress",
                cost: 8, attack: 10, health: 9);
            cardList.AddCard(name: "Crna rupa", clientSideImage: "black-hole",
                cost: 10, attack: 15, health: 15);

        }
    }
}
