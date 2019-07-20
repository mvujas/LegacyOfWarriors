using GameServer.Logic;
using GameServer.Logic.Validation;
using Remote.Implementation;
using Remote.Interface;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling.Handlers
{
    public class RegistrationHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            RegistrationRequest registrationRequest = request as RegistrationRequest;
            ServerSideTokenIdentity identity = token.info as ServerSideTokenIdentity;
            try
            {
                if (identity.LastlyFetchedUser != null)
                {
                    return new RegistrationResponse
                    {
                        Successfulness = false,
                        Message = "Ne mozes se registrovati dok si ulogovan"
                    };
                }

                UserLogic.RegisterUser(registrationRequest.Username, registrationRequest.Password);

                return new RegistrationResponse
                {
                    Successfulness = true
                };
            }
            catch (InvalidLogicDataException e)
            {
                return new RegistrationResponse
                {
                    Successfulness = false,
                    Message = e.Message
                };
            }
        }
    }
}