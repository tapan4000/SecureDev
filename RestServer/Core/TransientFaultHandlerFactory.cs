using RestServer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Entities.Core;
using RestServer.Entities.Enums;
using System.Collections.Concurrent;
using RestServer.IoC;
using RestServer.IoC.Interfaces;

namespace RestServer.Core
{
    public class TransientFaultHandlerFactory : ITransientFaultHandlerFactory
    {
        private IDependencyContainer dependencyContainer;

        public TransientFaultHandlerFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        private ConcurrentDictionary<string, ITransientErrorRetryPolicy> cachedRetryPolicyList = new ConcurrentDictionary<string, ITransientErrorRetryPolicy>();

        public ITransientErrorRetryPolicy GetRetryPolicy(TargetSystemEnum targetSystem, RetryTypeEnum retryType, RetrySetting retrySetting)
        {
            var cachePolicyKey = string.Concat(targetSystem, CoreConstants.UnderscoreConcatenator, retryType);
            ITransientErrorRetryPolicy cachedPolicy;
            if (!cachedRetryPolicyList.TryGetValue(cachePolicyKey, out cachedPolicy))
            {
                cachedPolicy = this.CreateRetryPolicy(retryType, retrySetting);
                cachedRetryPolicyList[cachePolicyKey] = cachedPolicy;
            }

            return cachedPolicy;
        }

        private ITransientErrorRetryPolicy CreateRetryPolicy(RetryTypeEnum retryType, RetrySetting retrySetting)
        {
            var retryPolicyConvention = string.Format(ConventionConstants.RetryTypeBasedPolicyConvention, retryType);
            var retryPolicy = this.dependencyContainer.Resolve<ITransientErrorRetryPolicy>(retryPolicyConvention);
            retryPolicy.Initialize(retrySetting);
            return retryPolicy;
        }
    }
}
