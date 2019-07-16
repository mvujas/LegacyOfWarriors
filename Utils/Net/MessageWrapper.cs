using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.Net
{
    public class MessageWrapper
    {
        public AsyncUserToken UserToken { get; set; }
        public byte[] Message { get; set; }
    }
}
