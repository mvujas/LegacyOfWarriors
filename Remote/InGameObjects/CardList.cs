using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.InGameObjects
{
    [Serializable]
    public class CardList : IRemoteObject
    {
        public string Vesion { get; private set; }

        public CardList(string vesion)
        {
            Vesion = vesion ?? throw new ArgumentNullException(nameof(vesion));
        }

        private List<Card> m_cards = new List<Card>();

        public void AddCard(string name, string clientSideImage, int cost, int attack, int health)
        {
            m_cards.Add(new Card(
                id: m_cards.Count,
                name,
                clientSideImage,
                cost,
                attack,
                health
            ));
        }

        public Card GetCardById(int id)
        {
            if(id < 0 || id >= m_cards.Count)
            {
                return null;
            }
            return m_cards[id];
        }
    }
}
