using GameServer.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public static class Config
    {
        public static NameValueCollection DbConfig { get; set; }

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
