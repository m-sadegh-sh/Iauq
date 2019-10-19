using System;
using System.Linq;
using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class ExceptionProvider : LogProviderBase<Exception>
    {
        public ExceptionProvider(Exception instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "an exception occured when processing request from user \"{0}\": (message: \"{1}\").",
                    Identity, Instance.Message);
            log.Stack = Instance.StackTrace.Replace("\n", "<br/>").Replace("\t",
                                                                           string.Join("",
                                                                                       Enumerable.Repeat("&nbsp;", 8)));
            log.ExceptionType = Instance.GetType().FullName;
            log.Level = LogLevel.Exception;
        }
    }
}