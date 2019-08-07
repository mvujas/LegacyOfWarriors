using System;
using Utils.Interface;
using Utils.Net;
using Remote.Implementation;
using Remote.Interface;

namespace Utils.Remote
{
    public abstract class LogicRouter : IMessageHandlingContainer
    {
        public IRemoteSender Sender { get; set; }

        protected abstract RemoteRequestMapper GetRequestMapper();

        public void OnMessageError(AsyncUserToken userToken)
        {
            Sender.Send(userToken, new RequestProcessingError());
        }

        public void OnMessageReceived(MessageWrapper messageWrapper)
        {
            try
            {
                HandleRequest(messageWrapper);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while processing: " + e);
                OnMessageError(messageWrapper.UserToken);
            }
        }

        private void HandleRequest(MessageWrapper messageWrapper)
        {
            IRemoteObject request =
                    SeriabilityUtils.ByteArrayToObject<IRemoteObject>(messageWrapper.Message);
            IRemoteObject response = GetRequestMapper().Handle(messageWrapper.UserToken, request);
            if (response != null)
            {
                Sender.Send(messageWrapper.UserToken, response);
            }
        }
    }
}
