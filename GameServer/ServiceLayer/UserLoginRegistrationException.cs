using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.ServiceLayer
{
    class UserLoginRegistrationException : Exception
    {
        public UserLoginRegistrationException(string message)
            : base(message)
        {

        }
    }
}
