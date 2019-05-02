using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public User(int id, string username, string passwordHash)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
        }  
    }
}
