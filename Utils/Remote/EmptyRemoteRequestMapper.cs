using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;

namespace Utils.Remote
{
    public class EmptyRemoteRequestMapper : RemoteRequestMapper
    {
        private Dictionary<Type, RequestHandler> m_mapper = new Dictionary<Type, RequestHandler>();

        protected override Dictionary<Type, RequestHandler> GetMapperDictionary()
        {
            return m_mapper;
        }

        protected override IRemoteObject InvalidTypeRepsonse()
        {
            return null;
        }
    }
}
