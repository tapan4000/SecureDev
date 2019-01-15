using RestServer.Entities.Core;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Core.Interfaces
{
    public interface ITransientFaultHandlerFactory
    {
        ITransientErrorRetryPolicy GetRetryPolicy(TargetSystemEnum targetType, RetryTypeEnum retryType, RetrySetting retrySetting);
    }
}
