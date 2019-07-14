using Dapper;
using System.IO;
using System.Text.RegularExpressions;

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
