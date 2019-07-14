using MySql.Data.MySqlClient;
using System;

namespace GameServer.Logic
{
    internal static class Extensions {
        public static bool IsDuplicateEntry(this MySqlException e)
        {
            return e.Message.StartsWith("duplicate entry", 
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}