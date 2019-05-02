using System;
using GameServer;
using GameServer.Database;
using MySql.Data.MySqlClient;
using GameServer.Model;
using GameServer.DataAccessLayer;
using GameServer.ServiceLayer;

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

            try
            {
                UserService.VerifyUser("mvujas", "nekaGlupavaSifra");
                UserService.RegisterUser("mvujas", "nekaGlupavaSifra");
            }
            catch(UserLoginRegistrationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            UserDao.GetInstance().GetAllUsers().ForEach(PrintUser);
            

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
