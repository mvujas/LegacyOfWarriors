using System;
using GameServer;
using GameServer.Database;
using MySql.Data.MySqlClient;
using GameServer.Model;
using GameServer.DataAccessLayer;

namespace GameServer
{
    class Program
    {
        static void PrintUser(User u)
        {
            Console.WriteLine(String.Format("{0} {1} {2}", u.Id, u.Username, u.PasswordHash));
        }

        static void Main(string[] args)
        {
            MySQLDatabaseManager.SetInstance(new MySQLDatabaseManager("localhost", "root", "game_db", 3306, ""));

            MySQLDatabaseManager manager = MySQLDatabaseManager.GetInstance();
            
            UserDao userDao = UserDao.GetInstance();
            userDao.GetAllUsers().ForEach(PrintUser);

            Console.WriteLine(" -- break -- ");

            Console.WriteLine(userDao.AddUser("perica", "marica"));
                
            userDao.GetAllUsers().ForEach(PrintUser);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
