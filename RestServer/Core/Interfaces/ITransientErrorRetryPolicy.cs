using RestServer.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Core.Interfaces
{
    public interface ITransientErrorRetryPolicy
    {
        void Initialize(RetrySetting retrySetting);

        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> retryDelegate);
    }
}
