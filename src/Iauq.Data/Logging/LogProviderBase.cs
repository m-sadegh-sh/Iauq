using System;
using System.Web;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using StructureMap;

namespace Iauq.Data.Logging
{
    public abstract class LogProviderBase<TType>
    {
        protected readonly TType Instance;
        private readonly HttpRequest _request;

        protected LogProviderBase(TType instance)
        {
            Instance = instance;
            _request = HttpContext.Current.Request;
        }

        protected string Identity
        {
            get { return _request.IsAuthenticated ? HttpContext.Current.User.Identity.Name : _request.UserHostAddress; }
        }

        public Log Provide()
        {
            
            var log = new Log
                          {
                              LogDate = DateTime.Now,
                              IpAddress = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress(),
                              RequestUrl = _request.Url.ToString(),
                              ReferUrl = _request.UrlReferrer != null ? _request.UrlReferrer.ToString() : null,
                          };

            Inject(log);

            return log;
        }

        protected abstract void Inject(Log log);
    }
}