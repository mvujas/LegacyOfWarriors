using ClientUtils;
using ProjectLevelConfig;
using Remote;
using Remote.Implementation;
using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Utils.Interface;
using Utils.Net;
using Utils.Remote;

namespace TestClient
{
    class LoginResponseHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            LoginResponse loginResponse = request as LoginResponse;

            Console.WriteLine("Odgovor na zahtev za prijavljivanje:");
            Console.WriteLine($"Uspesnost: {loginResponse.Successfulness}\nPoruka: {loginResponse.Message}");

            return null;
        }
    }

    class DefaultRemoteRequestMapper : RemoteRequestMapper
    {
        private Dictionary<Type, RequestHandler> m_mapper = new Dictionary<Type, RequestHandler>
        {
            [typeof(LoginResponse)] = new LoginResponseHandler()
        };

        protected override Dictionary<Type, RequestHandler> GetMapperDictionary()
        {
            return m_mapper;
        }
        protected override IRemoteObject InvalidTypeRepsonse()
        {
            return null;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                EndPointConfig.HOST,
                EndPointConfig.PORT
            );

            GameClientSpec spec = new GameClientSpec
            {
                bufferSize = 100,
                endPoint = endPoint
            };

            GameClient gameClient = new GameClient(spec, new DefaultRemoteRequestMapper());

            gameClient.Start(
                () => Console.WriteLine("Uspesno povezan"),
                () => Console.WriteLine("Nije uspesno povezan")
            );

            gameClient.Send(new LoginRequest
            {
                Username="mvujas",
                Password="pera1234"
            });

            Console.ReadKey();
        }
    }
}
