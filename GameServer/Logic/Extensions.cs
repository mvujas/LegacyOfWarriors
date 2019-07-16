using MySql.Data.MySqlClient;
using System;

namespace GameServer.Logic
{
    internal static class Extensions {
        public static bool IsDuplicateEntry(this MySqlException e)
        {
            const string duplicateEntryStarting = "duplicate entry";
            return e.Message.StartsWith(duplicateEntryStarting,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}