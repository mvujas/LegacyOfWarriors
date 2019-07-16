using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Net
{
    public interface EventHandlingContainer
    {
        void OnUserConnect(AsyncUserToken userToken);
        void OnUserDisconnect(AsyncUserToken userToken);
        void OnMessageReceived(MessageWrapper message);
        void OnMessageError(AsyncUserToken userToken);
    }
}
