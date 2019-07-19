using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Net;
using Remote.Implementation;
using Remote.Interface;
using Utils.Logic;

namespace GameServer.GameServerLogic
{
    public abstract class LogicRouter : IEventHandlingContainer
    {
        public GameServer Server { get; set; }

        protected abstract RemoteRequestMapper GetRequestMapper();

        public void OnMessageError(AsyncUserToken userToken)
        {
            Server.Send(userToken, new RequestProcessingError());
        }

        public void OnMessageReceived(MessageWrapper messageWrapper)
        {
            try
            {
                HandleRequest(messageWrapper);
            }
            catch (Exception)
            {
                OnMessageError(messageWrapper.UserToken);
            }
        }

        private void HandleRequest(MessageWrapper messageWrapper)
        {
            IRemoteObject request =
                    Utils.SeriabilityUtils.ByteArrayToObject<IRemoteObject>(messageWrapper.Message);
            IRemoteObject response = GetRequestMapper().Handle(request);
            if (response != null)
            {
                Server.Send(messageWrapper.UserToken, response);
            }
        }

        public abstract void OnUserConnect(AsyncUserToken userToken);

        public abstract void OnUserDisconnect(AsyncUserToken userToken);
    }
}
