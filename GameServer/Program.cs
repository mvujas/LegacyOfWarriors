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
        public void OnMessageError(AsyncUserToken userToken)
        {
            throw new NotImplementedException();
        }

        public void OnMessageReceived(MessageWrapper message)
        {
            throw new NotImplementedException();
        }

        public void OnUserConnect(AsyncUserToken userToken)
        {
            throw new NotImplementedException();
        }

        public void OnUserDisconnect(AsyncUserToken userToken)
        {
            throw new NotImplementedException();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            SocketServer socketServer = new SocketServer(
                100,
                100,
                new CustomEventHandlingContainer()
            );

            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                SocketServerConfig.HOST,
                SocketServerConfig.PORT
            );

            socketServer.Start(endPoint);


        }
    }
}
