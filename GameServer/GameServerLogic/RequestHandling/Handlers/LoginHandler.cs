using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Implementation;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class LoginHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            LoginRequest loginRequest = request as LoginRequest;

            Console.WriteLine("Login request!");

            return loginRequest;
        }
    }
}
