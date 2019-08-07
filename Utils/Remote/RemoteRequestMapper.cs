using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote.Interface;
using Utils.Net;

namespace Utils.Remote
{
    public abstract class RemoteRequestMapper
    {
        protected abstract IRemoteObject InvalidTypeRepsonse(IRemoteObject remoteObject);
        protected abstract Dictionary<Type, RequestHandler> GetMapperDictionary();

        public IRemoteObject Handle(AsyncUserToken token, IRemoteObject remoteObject)
        {
            Dictionary<Type, RequestHandler> handleMapper = GetMapperDictionary();
            if(handleMapper == null)
            {
                throw new InvalidOperationException();
            }
            RequestHandler handler;
            if(handleMapper.TryGetValue(remoteObject.GetType(), out handler))
            {
                return handler.Handle(token, remoteObject);
            }
            return InvalidTypeRepsonse(remoteObject);
        }
    }
}
