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
using Remote.Interface;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Config.Prepare();

            Initializer.Initialize();

            var handler = new CardGameLogicRouter();
            IPEndPoint endPoint = NetUtils.CreateEndPoint(EndPointConfig.HOST, EndPointConfig.PORT);

            GameServerSpec gameServerSpec = new GameServerSpec
            {
                endPoint = endPoint,
                eventConsumingAgents = 10,
                maxServerConnections = 50,
                socketServerBackLog = 10,
                socketServerBufferSize = 50
            };

            var gameServer = new GameServer.GameServerLogic.GameServer(gameServerSpec, handler);

            Config.GameServer = gameServer;

            gameServer.Start();
        }
    }
}
