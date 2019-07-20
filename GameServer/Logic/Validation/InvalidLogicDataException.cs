using System;
using System.Runtime.Serialization;

namespace GameServer.Logic.Validation
{
    public class InvalidLogicDataException : Exception
    {
        public InvalidLogicDataException()
        {
        }

        public InvalidLogicDataException(string message) : base(message)
        {
        }

        public InvalidLogicDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLogicDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
