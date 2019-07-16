using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Utils.Net
{
    public static class NetUtils
    {
        const string LOCALHOST_IPADDRESS = "127.0.0.1";

        public static IPEndPoint CreateEndPoint(string host, int port)
        {
            if(host.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
            {
                host = LOCALHOST_IPADDRESS;
            }
            return new IPEndPoint(IPAddress.Parse(host), port);
        }
    }
}
