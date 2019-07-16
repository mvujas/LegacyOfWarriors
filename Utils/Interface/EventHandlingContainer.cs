using System;
using System.Collections.Generic;
using System.Text;
using Utils.Net;

namespace Utils.Interface
{
    public interface IEventHandlingContainer
    {
        void OnUserConnect(AsyncUserToken userToken);
        void OnUserDisconnect(AsyncUserToken userToken);
        void OnMessageReceived(MessageWrapper message);
        void OnMessageError(AsyncUserToken userToken);
    }
}
