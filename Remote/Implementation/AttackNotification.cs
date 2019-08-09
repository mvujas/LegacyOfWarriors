using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class AttackNotification : IRemoteObject
    {
        public int AttackingPlayer { get; set; }
        public int? AttackingUnit { get; set; }
        public int TargetPlayer { get; set; }
        public int? TargetUnit { get; set; }
        public int AttackerRemainingHealth { get; set; }
        public int TargetRemainingHealth { get; set; }
    }
}
