using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Utils.GameLogicUtils
{
    public class LogicExecutionException : Exception
    {
        public LogicExecutionException()
        {
        }

        public LogicExecutionException(string message) : base(message)
        {
        }

        public LogicExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogicExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
