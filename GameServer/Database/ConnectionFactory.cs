using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace GameServer.Database
{
    public static class ConnectionFactory
    {
        public static string ConnectionString { get; private set; }

        public static void SetConnectionString(
            string server, string user, string db, int port, string password)
        {
            ConnectionString = 
                $"server={server};user={user};database={db};port={port};password={password}";
        }

        public static IDbConnection GetConnection()
        {
            if(ConnectionString == null)
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }
            return new MySqlConnection(ConnectionString);
        }
    }
}
