using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameServer.Database
{
    public delegate void TransactionBlock(MySqlConnection connection, MySqlTransaction transaction);

    public class MySQLDatabaseManager
    {
        #region SINGLETON

        private static MySQLDatabaseManager manager = null;

        public static MySQLDatabaseManager GetInstance()
        {
            if (manager == null)
            {
                throw new InvalidOperationException("Database manager is not set");
            }
            return manager;
        }

        public static void SetInstance(MySQLDatabaseManager manager)
        {
            MySQLDatabaseManager.manager = manager;
        }

        #endregion

        private string connectionString;

        public MySQLDatabaseManager(string server, string user, string db, int port, string password)
        {
            connectionString = $"server={server};user={user};database={db};port={port};password={password}";
        }

        public Boolean RunTransaction(TransactionBlock transactionBlock)
        {
            Boolean ok = true;
            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlTransaction transaction = null;
                try
                {
                    transaction = connection.BeginTransaction();
                    transactionBlock(connection, transaction);
                    transaction.Commit();
                }
                catch(MySqlException ex)
                {
                    ok = false;
                    Console.WriteLine(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (MySqlException ex1)
                    {
                        Console.WriteLine(ex1);
                    }
                }
            }
            return ok;
        }
    }
}
