using GameServer.Logic;
using GameServer.Repositories;
using System;
using GameServer.Net;
using Utils.Net;
using System.Net;
using ProjectLevelConfig;

namespace GameServer
{
    class CustomEventHandlingContainer : EventHandlingContainer
    {
        public SocketServer server;

        public void OnMessageError(AsyncUserToken userToken)
        {
            
        }

        public void OnMessageReceived(MessageWrapper message)
        {
            
        }

        public void OnUserConnect(AsyncUserToken userToken)
        {
            Console.WriteLine("New connection!");
            server.Send(userToken, "Zdravo svete!");
        }

        public void OnUserDisconnect(AsyncUserToken userToken)
        {
            Console.WriteLine("Connection over!");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            var abc = new CustomEventHandlingContainer();

            SocketServer socketServer = new SocketServer(
                100,
                100,
                abc
            );

            abc.server = socketServer;



            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                SocketServerConfig.HOST,
                SocketServerConfig.PORT
            );

            socketServer.Start(endPoint);


        }
    }
}
