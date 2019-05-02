using System;
using System.Collections.Generic;
using System.Data;
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
                catch(MySqlException)
                {
                    ok = false;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (MySqlException)
                    {
                    }
                }
            }
            return ok;
        }

        public IEnumerable<IDataRecord> SelectData(string query, params MySqlParameter[] pars)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                connection.Open();

                if (pars != null)
                {
                    foreach (MySqlParameter p in pars)
                    {
                        cmd.Parameters.Add(p);
                    }
                }

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }
    }
}
