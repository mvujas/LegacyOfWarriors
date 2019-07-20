using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUtils
{
    public interface IConnectionEventHandler
    {
        void OnSuccessfulConnection();
        void OnFailedConnection();
        void OnDisconnect();
    }
}
