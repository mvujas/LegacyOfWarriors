using Remote.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.Implementation
{
    [Serializable]
    public class RegistrationResponse : IRemoteObject
    {
        public bool Successfulness { get; set; }
        public string Message { get; set; }
    }
}
