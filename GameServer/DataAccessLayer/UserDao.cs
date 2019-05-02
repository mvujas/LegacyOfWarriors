using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using GameServer.Database;
using MySql.Data.MySqlClient;

namespace GameServer.DataAccessLayer
{
    public class UserDao
    {
        #region SINGLETON

        private static UserDao instance = new UserDao();

        public static UserDao GetInstance()
        {
            return instance;
        }

        #endregion

        private Dictionary<long, User> usersById = new Dictionary<long, User>();
        private Dictionary<string, User> usersByUsername = new Dictionary<string, User>();
        public UserDao()
        {
            MySQLDatabaseManager databaseManager = MySQLDatabaseManager.GetInstance();
            foreach (var row in databaseManager.SelectData("SELECT id, username, password FROM User"))
            {
                User user = new User(row.GetInt64(0), row.GetString(1), row.GetString(2));
                AddUserToLists(user);
            }
        }

        private void AddUserToLists(User user)
        {
            if(user != null)
            {
                usersById.Add(user.Id, user);
                usersByUsername.Add(user.Username, user);
            }
        }

        public List<User> GetAllUsers()
        {
            return usersById.Values.ToList<User>();
        }

        public User GetUserById(long id)
        {
            User user;
            usersById.TryGetValue(id, out user);
            return user;
        }

        public User GetUserByUsername(string username)
        {
            User user;
            usersByUsername.TryGetValue(username, out user);
            return user;
        }

        public Boolean AddUser(string username, string passwordHash)
        {
            long lastInsertedId = 0;
            Boolean success = MySQLDatabaseManager.GetInstance().RunTransaction((connection, transaction) =>
            {
                using(MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO User(username, password) VALUES (@username, @password)", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add(new MySqlParameter("@username", username));
                    cmd.Parameters.Add(new MySqlParameter("@password", passwordHash));

                    cmd.ExecuteNonQuery();

                    lastInsertedId = cmd.LastInsertedId;
                }
            });

            if(success)
            {
                User user = new User(lastInsertedId, username, passwordHash);
                AddUserToLists(user);
            }

            return success;
        }
    }
}
