using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.InGameObjects
{
    [Serializable]
    public class Card : IRemoteObject
    {
        public Card(int id, string name, string clientSideImage, int cost, int attack, int health)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ClientSideImage = clientSideImage ?? throw new ArgumentNullException(nameof(clientSideImage));
            Cost = cost;
            Attack = attack;
            Health = health;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ClientSideImage { get; private set; }
        public int Cost { get; private set; }
        public int Attack { get; private set; }
        public int Health { get; private set; }
    }
}
