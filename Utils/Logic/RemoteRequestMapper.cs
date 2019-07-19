using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;

namespace Utils.Logic
{
    public abstract class RemoteRequestMapper
    {
        protected abstract IRemoteObject InvalidTypeRepsonse();
        protected abstract Dictionary<Type, RequestHandler> GetMapperDictionary();

        public IRemoteObject Handle(IRemoteObject remoteObject)
        {
            Dictionary<Type, RequestHandler> handleMapper = GetMapperDictionary();
            if(handleMapper == null)
            {
                throw new InvalidOperationException();
            }
            RequestHandler handler;
            if(handleMapper.TryGetValue(remoteObject.GetType(), out handler))
            {
                return handler.Handle(remoteObject);
            }
            return InvalidTypeRepsonse();
        }
    }
}
