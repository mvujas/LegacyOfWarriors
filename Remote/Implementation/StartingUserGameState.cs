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
    public class StartingUserGameState : IRemoteObject
    {
        public int PlayerIndex { get; set; }
        public LinkedList<CardInGame> StartingDeck { get; set; }
    }
}
