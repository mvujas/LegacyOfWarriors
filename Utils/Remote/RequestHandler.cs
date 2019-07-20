using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace Utils.Remote
{
    public interface RequestHandler
    {
        IRemoteObject Handle(AsyncUserToken token, IRemoteObject request);
    }
}
