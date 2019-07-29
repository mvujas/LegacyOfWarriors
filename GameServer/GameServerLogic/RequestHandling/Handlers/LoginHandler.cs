using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Logic;
using GameServer.Logic.Validation;
using Remote.Implementation;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;
using GameServer.GameServerLogic.Lists;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class LoginHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            LoginRequest loginRequest = request as LoginRequest;
            ServerSideTokenIdentity identity = token.info as ServerSideTokenIdentity;
            try
            {
                return TryToLogin(identity, loginRequest.Username, loginRequest.Password);
            }
            catch(InvalidLogicDataException e)
            {
                return new LoginResponse
                {
                    Successfulness = false,
                    Message = e.Message
                };
            }
        }

        private LoginResponse TryToLogin(ServerSideTokenIdentity identity, string username, string password)
        {
            lock(identity)
            {
                if (identity.MatchmakingStatus != UserMatchmakingStatus.NON_LOGGED)
                {
                    return new LoginResponse
                    {
                        Successfulness = false,
                        Message = "Vec si ulogovan"
                    };
                }

                var user = UserLogic.GetUserByLoginInfo(username, password);

                if(!UserConnectionList.GetInstance().TryToLoginUser(user, identity))
                {
                    return new LoginResponse
                    {
                        Successfulness = false,
                        Message = "Dati korisnik je vec ulogovan"
                    };
                }

                identity.LastlyFetchedUser = user;
                identity.MatchmakingStatus = UserMatchmakingStatus.LOBBY;

                return new LoginResponse
                {
                    Successfulness = true
                };
            }
        }
    }
}
