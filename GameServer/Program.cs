using System;
using GameServer;
using GameServer.Database;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MySQLDatabaseManager.SetInstance(new MySQLDatabaseManager("localhost", "root", "game_db", 3306, ""));

            MySQLDatabaseManager manager = MySQLDatabaseManager.GetInstance();

            Console.WriteLine(manager.RunTransaction((connection, transaction) =>
            {
                using(MySqlCommand cmd = new MySqlCommand("INSERT INTO Users(id, username, password) VALUES (16, 'a1','')", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Users(id, username, password) VALUES (16, 'a2','')", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
            }));

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
