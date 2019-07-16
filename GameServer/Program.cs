using GameServer.Logic;
using GameServer.Repositories;
using System;
using GameServer.Net;
using Utils.Net;
using System.Net;
using ProjectLevelConfig;
using GameServer.GameServerLogic;
using GameServer.GameServerLogic.ConcurrentScheduling;
using System.Collections.Generic;
using Utils.Interface;
using System.Text;

namespace GameServer
{
    class CustomEventHandlingContainer : IEventHandlingContainer
    {
        public GameServer.GameServerLogic.GameServer server;

        public void OnMessageError(AsyncUserToken userToken)
        {
            Console.WriteLine("Greska u obradi!");
        }

        public void OnMessageReceived(MessageWrapper message)
        {
            Console.WriteLine("Message length: " + message.Message.Length);
            Console.WriteLine("New message: " + Encoding.ASCII.GetString(message.Message));
            server.Receive(message.UserToken);
        }

        public void OnUserConnect(AsyncUserToken userToken)
        {
            Console.WriteLine("New connection!");
            server.Receive(userToken);
        }

        public void OnUserDisconnect(AsyncUserToken userToken)
        {
            Console.WriteLine("New disconnection!");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            var handler = new CustomEventHandlingContainer();

            GameServerSpec gameServerSpec = new GameServerSpec
            {
                endPoint = NetUtils.CreateEndPoint(SocketServerConfig.HOST, SocketServerConfig.PORT),
                eventConsumingAgents = 2,
                maxServerConnections = 50,
                socketServerBackLog = 10,
                socketServerBufferSize = 10,
                eventHandler = handler
            };

            var gameServer = new GameServer.GameServerLogic.GameServer(gameServerSpec);

            handler.server = gameServer;

            gameServer.Start();

            Console.ReadKey();


        }
    }
}
