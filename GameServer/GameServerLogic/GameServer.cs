using GameServer.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.GameServerLogic.EventHandling;
using Utils.Net;
using Remote.Interface;
using Utils.Interface;

namespace GameServer.GameServerLogic
{
    public class GameServer : IRemoteSender
    {
        private GameServerSpec m_spec;
        private SocketServer m_socketServer;
        private ConcurrentSchedulingEventHandlingContainer m_concurrentSchedulingEventHandlingContainer;

        public GameServer(GameServerSpec spec, ServerSideLogicRouter router = null)
        {
            if(router == null)
            {
                router = new CardGameLogicRouter();
            }
            router.Sender = this;
            m_spec = spec;
            m_concurrentSchedulingEventHandlingContainer = 
                new ConcurrentSchedulingEventHandlingContainer(router, spec.eventConsumingAgents);
            m_socketServer = new SocketServer(
                spec.maxServerConnections,
                spec.socketServerBufferSize,
                m_concurrentSchedulingEventHandlingContainer,
                spec.socketServerBackLog);
        }

        public void Start()
        {
            m_concurrentSchedulingEventHandlingContainer.StartQueue();
            m_socketServer.Start(m_spec.endPoint);

            Console.WriteLine("Press any key to terminate game server process...");
            Console.ReadKey();
            Environment.Exit(1);
        }

        public void Send(AsyncUserToken token, IRemoteObject remoteObject)
        {
            byte[] message = Utils.SeriabilityUtils.ObjectToByteArray(remoteObject);
            m_socketServer.Send(token, message);
        }

        public void Send(AsyncUserToken token, byte[] message)
        {
            m_socketServer.Send(token, message);
        }

        public void Send(AsyncUserToken token, string message)
        {
            m_socketServer.Send(token, message);
        }

        public void Receive(AsyncUserToken token)
        {
            m_socketServer.Receive(token);
        }

    }
}
