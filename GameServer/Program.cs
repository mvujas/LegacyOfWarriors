using System;
using System.Collections.Specialized;
using GameServer;
using GameServer.Database;
using MySql.Data.MySqlClient;
using GameServer.Model;
using GameServer.DataAccessLayer;
using GameServer.ServiceLayer;
using System.Configuration;

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
            NameValueCollection dbConfig = ConfigurationManager.GetSection("databaseSettings") as NameValueCollection;
            MySQLDatabaseManager.SetInstance(new MySQLDatabaseManager(
                dbConfig["server"], dbConfig["user"], dbConfig["database"], Int32.Parse(dbConfig["port"]), dbConfig["password"]));

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
            
            Console.WriteLine(dbConfig["server"]);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
