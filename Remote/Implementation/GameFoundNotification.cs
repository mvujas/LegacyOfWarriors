using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class GameFoundNotification : IRemoteObject
    {
        public UserInfo EnemyInfo { get; set; }
        public int PlayersDeckSize { get; set; }
        public int EnemiesDeckSize { get; set; }
        public int PlayersHealth { get; set; }
        public int EnemiesHealth { get; set; }
    }
}
