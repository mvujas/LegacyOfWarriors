using GameServer.Database;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GameServer
{
    public static class Config
    {
        public static NameValueCollection DbConfig { get; set; }
        public static GameServerLogic.GameServer GameServer { get; set; }

        public static void Prepare()
        {
            PrepareDatabase();
        }

        private static void PrepareDatabase()
        {
            DbConfig = ConfigurationManager.GetSection("databaseSettings") as NameValueCollection;
            ConnectionFactory.SetConnectionString(
                DbConfig["server"], 
                DbConfig["user"], 
                DbConfig["database"], 
                Int32.Parse(DbConfig["port"]), 
                DbConfig["password"]);
        }

    }
}
