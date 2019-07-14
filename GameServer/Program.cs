using GameServer.Logic;
using GameServer.Repositories;
using System;

namespace GameServer
{
    public class Program
    {
        public static void print(Model.User user)
        {
            if(user == null)
            {
                Console.WriteLine("Nema korisnika");
            }
            else
            {
                Console.WriteLine(
                    $"User {user.Username} (id {user.Id}), password {user.PasswordHash}");
            }
        }

        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            UserRepository userRepo = new UserRepository();

            try
            {
                print(UserLogic.GetUserByLoginInfo("milos1", "perakralj123"));
                print(UserLogic.GetUserByLoginInfo("milos2", "perakralj123"));
                UserLogic.RegisterUser("milos2", "perakralj123");
            }
            catch(Exception e)
            {
                Console.WriteLine("Greska: {0}", e.Message);
            }

            print(UserLogic.GetUserById(1));

            Console.ReadKey();

        }
    }
}
