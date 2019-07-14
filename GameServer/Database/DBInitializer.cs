using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;

namespace GameServer.Database
{
    public static class DBInitializer
    {
        public static void InitializeDatabase(string initFileName)
        {
            string script = File.ReadAllText(initFileName);
            string[] commands = 
                Regex.Split(script, ";", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            using (var connection = ConnectionFactory.GetConnection())
            {
                connection.Open();
                foreach (var command in commands) {
                    if(command.Trim().Length > 0)
                    {
                        connection.Execute(command);
                    }
                }
            }
        }
    }
}
