using Remote.InGameObjects;
using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class CardPlayedNotification : IRemoteObject
    {
        public int PlayerIndex { get; set; }
        public CardInGame PlayedCard { get; set; }
        public int RemainingMana { get; set; }
    }
}
