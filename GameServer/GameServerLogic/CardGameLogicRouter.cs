using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Logic;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public class CardGameLogicRouter : LogicRouter
    {
        public override void OnUserConnect(AsyncUserToken userToken)
        {
            Console.WriteLine("User connected");
        }

        public override void OnUserDisconnect(AsyncUserToken userToken)
        {
            Console.WriteLine("User disconnected");
        }

        protected override RemoteRequestMapper GetRequestMapper()
        {
            throw new NotImplementedException();
        }
    }
}
