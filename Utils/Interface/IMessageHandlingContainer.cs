using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace Utils.Interface
{
    public interface IMessageHandlingContainer
    {
        void OnMessageReceived(MessageWrapper message);
        void OnMessageError(AsyncUserToken userToken);
    }
}
