using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Net;
using Utils.Remote;

namespace GameServer.GameServerLogic
{
    public abstract class ServerSideLogicRouter : LogicRouter, IEventHandlingContainer
    {
        public abstract void OnUserConnect(AsyncUserToken userToken);
        public abstract void OnUserDisconnect(AsyncUserToken userToken);
    }
}
