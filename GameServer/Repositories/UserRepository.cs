using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using Dapper;

namespace GameServer.Repositories
{
    public class UserRepository: BaseRepository<User, long>
    {
        public User GetByUsername(string username) {
            return StatefulConnectionRunningWrapper<User>(connection =>
            {
                return connection.QueryFirstOrDefault<User>(
                    @"SELECT * FROM User WHERE username = @Username",
                    new
                    {
                        Username = username
                    }
                );
            });
        }
    }
}
