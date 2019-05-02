using System;
using GameServer;
using GameServer.Database;
using MySql.Data.MySqlClient;
using GameServer.Model;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MySQLDatabaseManager.SetInstance(new MySQLDatabaseManager("localhost", "root", "game_db", 3306, ""));

            MySQLDatabaseManager manager = MySQLDatabaseManager.GetInstance();

            Console.WriteLine(hash);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
