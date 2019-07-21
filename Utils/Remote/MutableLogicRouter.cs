using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Net;

namespace Utils.Remote
{
    public class MutableLogicRouter : LogicRouter
    {
        public MutableLogicRouter(RemoteRequestMapper remoteRequestMapper)
        {
            RequestMapper = remoteRequestMapper;
        }

        public RemoteRequestMapper RequestMapper { get; set; }

        protected override RemoteRequestMapper GetRequestMapper()
        {
            if(RequestMapper == null)
            {
                throw new InvalidOperationException("Logic router without remote request mapper");
            }
            return RequestMapper;
        }
    }
}
