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
using Utils;
using Remote;
using System.Threading;

namespace GameServer
{
    class CustomEventHandlingContainer : IEventHandlingContainer
    {
        public GameServer.GameServerLogic.GameServer server;

        public void OnMessageError(AsyncUserToken userToken)
        {
            Console.WriteLine("Greska u obradi!");
        }

        private int messageCount = 0;

        public void OnMessageReceived(MessageWrapper message)
        {
            //Console.WriteLine("Message length: " + message.Message.Length);
            
            int brojPoruke = int.Parse(Encoding.ASCII.GetString(message.Message).Substring(20).Trim());
            if(brojPoruke % 100 == 0)
            {
                Console.WriteLine("Poruka broj " + brojPoruke);
            } 
            /*
            server.Send(message.UserToken, $"Poruka broj {broj} primljena!");*/

            //Objekat obj = SeriabilityUtils.ByteArrayToObject<Objekat>(message.Message);

            //Console.WriteLine("Pristigli objekat: " + obj);
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
            IPEndPoint endPoint = NetUtils.CreateEndPoint(EndPointConfig.HOST, EndPointConfig.PORT);

            GameServerSpec gameServerSpec = new GameServerSpec
            {
                endPoint = endPoint,
                eventConsumingAgents = 10,
                maxServerConnections = 50,
                socketServerBackLog = 10,
                socketServerBufferSize = 50,
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
