using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace Utils.Interface
{
    public interface IRemoteSender
    {
        void Send(AsyncUserToken userToken, IRemoteObject remoteObject);
    }
}
