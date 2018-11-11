using RestServer.Cache.Interfaces;
using RestServer.IoC;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class CacheStrategyHandler<T> : ICacheStrategyHandler<T>
    {
        private const string CacheConnectionStringParameterName = "cacheConnectionString";

        private const string DatabaseParameterName = "database";

        private readonly IDependencyContainer dependencyContainer;

        private readonly IEventLogger logger;

        private ICacheConfigurationHandler cacheConfigurationHandler;

        public CacheStrategyHandler(IDependencyContainer dependencyContainer, IEventLogger logger, ICacheMetadataProvider cacheHintStore, ICacheConfigurationHandler cacheConfigurationHandler)
        {
            this.dependencyContainer = dependencyContainer;
            this.logger = logger;
            this.cacheConfigurationHandler = cacheConfigurationHandler;
        }

        public Task<bool> ClearStoreCacheAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesKeyExistInStoreAsync(string key, string keyGroupName = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetFromStoreAsync(string key, string keyGroupName = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertOrUpdateInStoreAsync(string key, T entity, string keyGroupName = null, TimeSpan? expiry = default(TimeSpan?))
        {
            throw new NotImplementedException();
        }

        private async Task<ICacheStrategy<T>> GetCacheStrategyAsync(CacheMetadata cacheMetadata)
        {
            switch (cacheMetadata.CacheHint)
            {
                case CacheHint.DistributedCache:
                    var cacheConnectionString = await this.cacheConfigurationHandler.GetCacheConnectionStringAsync();
                    return this.dependencyContainer.Resolve<RedisCacheStrategy<T>>(
                        new DependencyParameterOverride(CacheConnectionStringParameterName, cacheConnectionString),
                        new DependencyParameterOverride(DatabaseParameterName, this.GetCacheDb(cacheMetadata.CacheArea)));
                case CacheHint.LocalCache:
                    return this.dependencyContainer.Resolve<InMemoryCacheStrategy<T>>();
                case CacheHint.None:
                    return default(ICacheStrategy<T>);
                default:
                    throw new NotSupportedException($"Cache Strategy is not supported for the storage hint type {cacheMetadata.CacheHint}");
            }
        }

        private int GetCacheDb(CacheArea cacheArea)
        {
            switch (cacheArea)
            {
                case CacheArea.Default:
                    return 0;
                case CacheArea.User:
                    return 1;
            }

            return 0;
        }
    }
}
