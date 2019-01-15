using RestServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class CacheTransientErrorRetryPolicy : TransientErrorRetryPolicyBase
    {
        protected override bool HandleResponse(object response)
        {
            // No need to handle response for transient errors.
            return false;
        }

        protected override bool HandleRetryException(Exception exception)
        {
            return this.IsTransient(exception, this.RetrySetting.TransientExceptionList);
        }

        private bool IsTransient(Exception exception, string[] exceptionList)
        {
            // Handle Operation Timeout
            var taskCancelledException = exception as TaskCanceledException;
            if (null != taskCancelledException)
            {
                return !taskCancelledException.CancellationToken.IsCancellationRequested;
            }

            var exceptionName = exception.GetType().FullName;
            return exceptionList.Any(s => exceptionName.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }
    }
}
