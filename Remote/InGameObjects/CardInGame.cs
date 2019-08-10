using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.InGameObjects
{
    [Serializable]
    public class CardInGame : IRemoteObject
    {
        [NonSerialized]
        public Card Card = null;
        public int CardId { get; set; }
        public int InGameId { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public int Cost { get; set; }
        public int LastAttackingTurn { get; set; }

        public void SetCard(Card card)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
            CardId = card.Id;
            InGameId = InGameId;
            Attack = card.Attack;
            Health = card.Health;
            Cost = card.Cost;
            LastAttackingTurn = -1;
        }
    }
}
