using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Logic
{
    public interface RequestHandler
    {
        IRemoteObject Handle(IRemoteObject request);
    }
}
