using GameServer.Database;
using System;
using System.IO;

namespace GameServer
{
    public static class Initializer
    {
        public static void Initialize()
        {
            try
            {
                DBInitializer.InitializeDatabase("init.sql");
                Console.WriteLine(" --- Baza podataka inicijalizovana");
                Console.WriteLine(" --- Inicijalizacija uspesno zavrsena");
            }
            catch(FileNotFoundException) {
                Console.WriteLine(
                    " * Greska: Ne moze se naci fajl za inicijalizaciju baze podataka");
            }
            
        }
    }
}
