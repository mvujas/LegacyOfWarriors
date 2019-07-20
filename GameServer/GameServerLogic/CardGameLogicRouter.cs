using GameServer.GameServerLogic.RequestHandling;
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
    public class CardGameLogicRouter : ServerSideLogicRouter
    {
        private RemoteRequestMapper m_requestMapper = new CardGameRequestMapper();

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
            return m_requestMapper;
        }
    }
}
