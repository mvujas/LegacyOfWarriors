using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Database;

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
