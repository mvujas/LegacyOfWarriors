using System;
using System.Collections.Generic;
using System.Text;
using Utils.Net;

namespace Utils.Interface
{
    public interface IEventHandlingContainer : IMessageHandlingContainer
    {
        void OnUserConnect(AsyncUserToken userToken);
        void OnUserDisconnect(AsyncUserToken userToken);
    }
}
