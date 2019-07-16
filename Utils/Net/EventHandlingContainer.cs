using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Net
{
    public interface EventHandlingContainer
    {
        void OnUserConnect(AsyncUserToken userToken);
        void OnUserDisconnect(AsyncUserToken userToken);
        void OnMessageReceived(MessageWrapper message);
        void OnMessageError(AsyncUserToken userToken);
    }
}
