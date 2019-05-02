using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class User
    {
        public long Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; set; }

        public User(long id, string username, string passwordHash)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
        }  
    }
}
