using ClientUtils;
using ProjectLevelConfig;
using Remote;
using Remote.Implementation;
using Remote.InGameObjects;
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
    class QueueEntryResponseHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            var queueResponse = request as QueueEntryResponse;

            Console.WriteLine("Odgovor na zahtev za red:");
            Console.WriteLine($"Uspesnost: {queueResponse.Successfulness}\nPoruka: {queueResponse.Message}");

            return null;
        }
    }

    class QueueExitResponseHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            var queueResponse = request as QueueExitResponse;

            Console.WriteLine("Odgovor na zahtev za izlazak iz reda:");
            Console.WriteLine($"Uspesnost: {queueResponse.Successfulness}\nPoruka: {queueResponse.Message}");

            return null;
        }
    }

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

    class CardResponseHandler : RequestHandler
    {
        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject request)
        {
            CardListResponse cardListResponse = request as CardListResponse;
            if (cardListResponse.UpToDate)
            {
                Console.WriteLine("Imam najnoviju verziju!");
            }
            else
            {
                CardList cardList = cardListResponse.CardList;
                Console.WriteLine("Stigla nova lista karata, verzija: " + cardList.Vesion);
            }

            return null;
        }
    }

    class DefaultRemoteRequestMapper : RemoteRequestMapper
    {
        private Dictionary<Type, RequestHandler> m_mapper = new Dictionary<Type, RequestHandler>
        {
            [typeof(LoginResponse)] = new LoginResponseHandler(),
            [typeof(CardListResponse)] = new CardResponseHandler(),
            [typeof(QueueEntryResponse)] = new QueueEntryResponseHandler(),
            [typeof(QueueExitResponse)] = new QueueExitResponseHandler()
        };

        protected override Dictionary<Type, RequestHandler> GetMapperDictionary()
        {
            return m_mapper;
        }
        protected override IRemoteObject InvalidTypeRepsonse()
        {
            Console.WriteLine("Invalid Type");
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

            gameClient.OnDisconnect = () => Console.WriteLine("I'm disconnecting :/");

            gameClient.Start(
                () => Console.WriteLine("Uspesno povezan"),
                () => Console.WriteLine("Nije uspesno povezan")
            );

            gameClient.Send(new LoginRequest
            {
                Username = "mvujas",
                Password = "pera12345"
            });

            gameClient.Send(new CardListRequest());

            gameClient.Send(new QueueEntryRequest {
                Deck = new int[]{ 0, 0, 0, 0 }
            });

            gameClient.Send(new QueueExitRequest());

            gameClient.Send(new QueueEntryRequest
            {
                Deck = new int[] { 0, 0, 0, 0 }
            });
            gameClient.Send(new QueueEntryRequest
            {
                Deck = new int[] { 0, 0, 0, 0 }
            });

            gameClient.Send(new QueueExitRequest());
            gameClient.Send(new QueueExitRequest());

            Console.WriteLine("Press any key to terminate game client...");
            Console.ReadKey();

            gameClient.Disconnect();

            Console.ReadKey();
        }
    }
}
