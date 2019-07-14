using GameServer.Database;
using System.Collections.Specialized;
using System.Configuration;
using System;
using GameServer.Repositories;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            UserRepository userRepo = new UserRepository();

            try {
                userRepo.Add(new Model.User
                {
                    Username = "milos",
                    PasswordHash = "abc"
                });
            }
            catch(Exception) {
                Console.WriteLine("Izuzetak");
            }

            var user = userRepo.GetById(1);

            Console.WriteLine(
                "Id: {0}\nUsername: {1}\nSifra: {2}", user.Id, user.Username, user.PasswordHash);

            Console.WriteLine("Get by username: ");
            user = userRepo.GetByUsername("pera");
            Console.WriteLine(
                "Id: {0}\nUsername: {1}\nSifra: {2}", user.Id, user.Username, user.PasswordHash);



            Console.WriteLine("Svi:");
            foreach(var user1 in userRepo.GetAll()) {
                Console.WriteLine(
                    "Id: {0}\nUsername: {1}\nSifra: {2}", user1.Id, user1.Username, user1.PasswordHash);
            }

            Console.ReadKey();
        }
    }
}
