using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Net
{
    public class MessageWrapper
    {
        public AsyncUserToken UserToken { get; set; }
        public byte[] Message { get; set; }
    }
}
