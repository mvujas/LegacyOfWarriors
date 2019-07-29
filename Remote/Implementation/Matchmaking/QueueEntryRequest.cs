using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class QueueEntryRequest : IRemoteObject
    {
        // Card ids of cards in deck
        public int[] Deck { get; set; }
    }
}
