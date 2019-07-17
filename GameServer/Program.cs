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
            //server.Send(message.UserToken, "Poruka primljena!");
        }

        public void OnUserConnect(AsyncUserToken userToken)
        {
            Console.WriteLine("New connection!");
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
            IPEndPoint endPoint = NetUtils.CreateEndPoint(SocketServerConfig.HOST, SocketServerConfig.PORT);

            GameServerSpec gameServerSpec = new GameServerSpec
            {
                endPoint = endPoint,
                eventConsumingAgents = 10,
                maxServerConnections = 50,
                socketServerBackLog = 10,
                socketServerBufferSize = 10,
                eventHandler = handler
            };

            var gameServer = new GameServer.GameServerLogic.GameServer(gameServerSpec);

            handler.server = gameServer;

            gameServer.Start();

            /*SocketServer socketServer = new SocketServer(10, 128, handler);

            handler.server = socketServer;

            socketServer.Start(endPoint);*/

            Console.ReadKey();


        }
    }
}
