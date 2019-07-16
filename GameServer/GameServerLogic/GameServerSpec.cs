using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;

namespace GameServer.GameServerLogic
{
    public struct GameServerSpec
    {
        public IPEndPoint endPoint;

        public int maxServerConnections;
        public int socketServerBufferSize;
        public int socketServerBackLog;
        public IEventHandlingContainer eventHandler;

        public int eventConsumingAgents;
    }
}
