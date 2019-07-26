using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.GameServerLogic
{
    public class MatchmakingException : Exception
    {
        public MatchmakingException()
        {
        }

        public MatchmakingException(string message) : base(message)
        {
        }

        public MatchmakingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MatchmakingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
