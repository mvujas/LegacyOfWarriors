using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.GameServerLogic.RequestHandling.Handlers;
using Remote.Implementation;
using Remote.Interface;
using Utils.Remote;

namespace GameServer.GameServerLogic.RequestHandling
{
    public class CardGameRequestMapper : RemoteRequestMapper
    {
        private Dictionary<Type, RequestHandler> m_mapping = new Dictionary<Type, RequestHandler> {
            [typeof(LoginRequest)] = new LoginHandler(),
            [typeof(RegistrationRequest)] = new RegistrationHandler(),
            [typeof(CardListRequest)] = new CardListRequestHandler(),
            [typeof(QueueEntryRequest)] = new QueueEntryRequestHandler(),
            [typeof(QueueExitRequest)] = new QueueExitRequestHandler()
        };

        private IRemoteObject m_invalidTypeHandlerResponse = new InvalidRequest();

        protected override Dictionary<Type, RequestHandler> GetMapperDictionary()
        {
            return m_mapping;
        }

        protected override IRemoteObject InvalidTypeRepsonse()
        {
            return m_invalidTypeHandlerResponse;
        }
    }
}
