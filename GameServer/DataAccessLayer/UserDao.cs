using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DataAccessLayer
{
    public class UserDao
    {
        #region SINGLETON

        private static UserDao instance = new UserDao();

        public static UserDao getInstance()
        {
            return instance;
        }

        #endregion

        
    }
}
