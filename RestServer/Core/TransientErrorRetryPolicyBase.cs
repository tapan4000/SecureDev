using RestServer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Entities.Core;
using Polly.Retry;
using RestServer.Entities.Enums;
using Polly;

namespace RestServer.Core
{
    public abstract class TransientErrorRetryPolicyBase : ITransientErrorRetryPolicy
    {
        protected RetrySetting RetrySetting { get; set; }

        protected RetryPolicy<object> RetryPolicy { get; set; }

        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> retryDelegate)
        {
            if(null != this.RetryPolicy)
            {
                var response = await this.RetryPolicy.ExecuteAsync(async () => await retryDelegate().ConfigureAwait(false)).ConfigureAwait(false);
                if (null != response && response is T)
                {
                    return (T)response;
                }
                else
                {
                    return default(T);
                }
            }

            return await retryDelegate().ConfigureAwait(false);
        }

        public void Initialize(RetrySetting retrySetting)
        {
            this.RetrySetting = retrySetting;
            this.RetryPolicy = this.GetRetryPolicy();
        }

        protected abstract bool HandleRetryException(Exception exception);

        protected abstract bool HandleResponse(object response);

        private RetryPolicy<object> GetRetryPolicy()
        {
            switch (this.RetrySetting.RetryStrategy)
            {
                case RetryStrategyEnum.Exponential:
                    return Policy.Handle<Exception>(this.HandleRetryException)
                        .OrResult<object>(this.HandleResponse)
                        .WaitAndRetryAsync(this.RetrySetting.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(this.RetrySetting.RetryIntervalInSeconds, retryAttempt)));
                case RetryStrategyEnum.Fixed:
                    return Policy.Handle<Exception>(this.HandleRetryException)
                        .OrResult<object>(this.HandleResponse)
                        .WaitAndRetryAsync(this.RetrySetting.RetryCount, retryAttempt => TimeSpan.FromSeconds(this.RetrySetting.RetryIntervalInSeconds));
                default:
                    return Policy.Handle<Exception>(this.HandleRetryException).OrResult<object>(this.HandleResponse).RetryAsync(this.RetrySetting.RetryCount);
            }
        }
    }
}
