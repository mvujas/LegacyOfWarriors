using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;
using Utils.Delegates;
using Utils.Interface;
using Utils.Net;
using Utils.Remote;

namespace ClientUtils
{
    public class GameClient : IRemoteSender
    {
        private SocketClient m_socketClient;
        private MutableLogicRouter m_logicRouter;
        private GameClientSpec m_spec;

        public Runnable OnDisconnect { set => m_socketClient.OnDisconnect = value; }

        public GameClient(GameClientSpec spec, RemoteRequestMapper remoteRequestMapper = null)
        {
            m_spec = spec;
            m_logicRouter = new MutableLogicRouter(remoteRequestMapper);
            m_logicRouter.Sender = this;
            m_socketClient = new SocketClient(m_logicRouter, m_spec.bufferSize);
        }

        public void ChangeRequestMapper(RemoteRequestMapper remoteRequestMapper)
        {
            m_logicRouter.RequestMapper = remoteRequestMapper;
        }

        public void Start(Runnable onSuccessfulConnecting = null, Runnable onFailedConnecting = null)
        {
            try
            {
                m_socketClient.Connect(m_spec.endPoint);
                onSuccessfulConnecting?.Invoke();
            }
            catch (Exception)
            {
                onFailedConnecting?.Invoke();
            }
        }

        public bool IsActive()
        {
            return m_socketClient.IsActive();
        }

        /// <param name="userToken">redundant parameter, a product of poor architecture</param>
        public void Send(AsyncUserToken _, IRemoteObject remoteObject)
        {
            Send(remoteObject);
        }

        public void Send(IRemoteObject remoteObject)
        {
            byte[] message = Utils.SeriabilityUtils.ObjectToByteArray(remoteObject);
            m_socketClient.Send(message);
        }

        public void Disconnect()
        {
            m_socketClient.Disconnect();
        }
    }
}
