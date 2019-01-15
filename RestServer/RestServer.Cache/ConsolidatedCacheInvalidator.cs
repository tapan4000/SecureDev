using RestServer.Cache.Core.Enums;
using RestServer.Cache.Interfaces;
using RestServer.Cache.Models;
using RestServer.Core.Interfaces;
using RestServer.Entities.Enums;
using RestServer.IoC;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    [IoCRegistration(IoCLifetime.Hierarchical)]
    public class ConsolidatedCacheInvalidator : IConsolidatedCacheInvalidator
    {
        private readonly List<CacheInvalidationData> consolidatedCacheList = new List<CacheInvalidationData>();

        private IEventLogger logger;

        private ICacheConfigurationHandler cacheConfigurationHandler;

        private ITransientFaultHandlerFactory transientFaultHandlerFactory;

        public ConsolidatedCacheInvalidator(IEventLogger logger, ICacheConfigurationHandler cacheConfigurationHandler, ITransientFaultHandlerFactory transientFaultHandlerFactory)
        {
            this.logger = logger;
            this.cacheConfigurationHandler = cacheConfigurationHandler;
            this.transientFaultHandlerFactory = transientFaultHandlerFactory;
        }

        public async Task invalidateAsync()
        {
            var cacheRetrySettings = await this.cacheConfigurationHandler.GetCacheRetrySetting().ConfigureAwait(false);
            if(null == cacheRetrySettings)
            {
                throw new Exception("Unable to fetch cache retry settings.");
            }

            var cacheRetryPolicy = this.transientFaultHandlerFactory.GetRetryPolicy(TargetSystemEnum.Cache, RetryTypeEnum.Cache, cacheRetrySettings);

            foreach(var cacheEntry in this.consolidatedCacheList)
            {
                try
                {
                    await cacheRetryPolicy.ExecuteWithRetryAsync(async () => await cacheEntry.CacheStrategy.DeleteAsync(cacheEntry.CacheKey)).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    this.logger.LogException($"Failed to clear cache for {cacheEntry.CacheKey}", ex);
                }
            }

            this.consolidatedCacheList.Clear();
        }

        public void Register(ICacheStrategy cacheStrategy, IList<string> cacheKeys)
        {
            foreach(var cacheKey in cacheKeys)
            {
                if(!this.IKeyAlreadyAdded(cacheKey))
                {
                    this.consolidatedCacheList.Add(new CacheInvalidationData(cacheStrategy, cacheKey));
                }
            }
        }

        public void Register(ICacheStrategy cacheStrategy, string cacheKey)
        {
            if(!this.IKeyAlreadyAdded(cacheKey))
            {
                this.consolidatedCacheList.Add(new CacheInvalidationData(cacheStrategy, cacheKey));
            }
        }

        private bool IKeyAlreadyAdded(string cacheKey)
        {
            if(this.consolidatedCacheList.Count > 0)
            {
                if (this.consolidatedCacheList.Any(record => record.CacheKey.Equals(cacheKey, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
