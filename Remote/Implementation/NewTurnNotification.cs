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
    public class NewTurnNotification : IRemoteObject
    {
        public int PlayerIndex { get; set; }
        public int Mana { get; set; }
        public CardInGame DrawnCard { get; set; }
        public CardDrawingOutcome DrawingOutcome { get; set; }
        public int FatiqueDamage { get; set; }
        public int RemainingHealth { get; set; }
    }
}
