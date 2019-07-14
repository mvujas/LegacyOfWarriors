using Dapper;
using GameServer.Model;
using System;

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

        public bool DoesUserExist(string username)
        {
            return StatefulConnectionRunningWrapper<bool>(connection =>
            {
                try
                {
                    connection.QueryFirst(
                        @"SELECT 1 FROM User WHERE username = @Username",
                        new
                        {
                            Username = username
                        }
                    );
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            });
        }
    }
}
