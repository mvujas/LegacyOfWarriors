using GameServer.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace GameServer.GameServerLogic.Lists
{
    public class UserConnectionList
    {
        private UserConnectionList() { }
        private static UserConnectionList instance = new UserConnectionList();
        public static UserConnectionList GetInstance()
        {
            return instance;
        }

        private ConcurrentDictionary<long, ServerSideTokenIdentity> m_userIdToTokenMapping =
            new ConcurrentDictionary<long, ServerSideTokenIdentity>();

        /// <returns>
        /// Vraca tacno ako dati korisnik nije ulogovan i postavlja da je dati identitet njegov, 
        /// ako je vec ulogovan vraca false
        /// </returns>
        public bool TryToLoginUser(User user, ServerSideTokenIdentity identity)
        {
            // Biti pazljiv gde se ovo koristi da ne dodje do dead-lock-a
            lock(identity)
            {
                bool result = m_userIdToTokenMapping.TryAdd(user.Id, identity);
                if(result)
                {
                    identity.LastlyFetchedUser = user;
                }
                return result;
            }
        }

        public void LogOutUserUnderIdentity(ServerSideTokenIdentity identity)
        {
            lock (identity)
            {
                if (identity.LastlyFetchedUser != null)
                {
                    m_userIdToTokenMapping.TryRemove(identity.LastlyFetchedUser.Id, out _);
                    identity.LastlyFetchedUser = null;
                }
            }
        }
    }
}
