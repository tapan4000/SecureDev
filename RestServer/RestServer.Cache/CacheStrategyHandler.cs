using RestServer.Cache.Interfaces;
using RestServer.Core.Extensions;
using RestServer.Core.Helpers;
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

        private ICacheStrategy<T> cacheStrategy;

        private CacheMetadata cacheMetadata;

        public CacheStrategyHandler(IDependencyContainer dependencyContainer, IEventLogger logger, ICacheMetadataProvider cacheMetadataProvider, ICacheConfigurationHandler cacheConfigurationHandler)
        {
            this.cacheMetadata = cacheMetadataProvider.GetCacheMetadata(typeof(T));
            this.dependencyContainer = dependencyContainer;
            this.logger = logger;
            this.cacheConfigurationHandler = cacheConfigurationHandler;
            this.cacheStrategy = this.GetCacheStrategyAsync(this.cacheMetadata);
        }

        public async Task<bool> ClearStoreCacheAsync()
        {
            if (null != this.cacheStrategy)
            {
                try
                {
                    return await this.cacheStrategy.ClearCacheAsync();
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            return true;
        }

        public async Task<bool> DoesKeyExistInStoreAsync(string key, string entityName = null)
        {
            if(null != this.cacheStrategy && !key.IsEmpty())
            {
                try
                {
                    var mergedKey = this.GetCacheKey(key, entityName);
                    return await this.cacheStrategy.DoesKeyExistAsync(mergedKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            return false;
        }

        public async Task<T> GetFromStoreAsync(string key, string entityName = null)
        {
            if (null != this.cacheStrategy && !key.IsEmpty())
            {
                try
                {
                    var mergedKey = this.GetCacheKey(key, entityName);
                    return await this.cacheStrategy.GetAsync(mergedKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            return default(T);
        }

        public async Task<bool> DeleteFromStoreAsync(string key, string entityName = null)
        {
            if (null != this.cacheStrategy && !key.IsEmpty())
            {
                try
                {
                    var mergedKey = this.GetCacheKey(key, entityName);
                    return await this.cacheStrategy.DeleteAsync(mergedKey).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            // Returning true as deletion if key would fail if the cachestrategy is null or key is null (which can happen if the key is not already populated in the cache).
            return true;
        }

        public async Task<bool> InsertOrUpdateInStoreAsync(string key, T entity, TimeSpan? expiry = null, string entityName = null)
        {
            if(null != this.cacheStrategy && !key.IsEmpty())
            {
                try
                {
                    if(null == expiry)
                    {
                        if (this.cacheMetadata.TimeToLiveInSeconds > 0)
                        {
                            expiry = TimeSpan.FromSeconds(this.cacheMetadata.TimeToLiveInSeconds.Value);
                        }
                        else
                        {
                            var isRedisCacheEnabled = await this.cacheConfigurationHandler.IsRedisCacheEnabled();

                            // In-Memory cache is always enabled by default.
                            bool isCacheEnabled = this.cacheMetadata.CacheHint == CacheHint.DistributedCache ?
                                isRedisCacheEnabled :
                                true;

                            if (isCacheEnabled) {
                                if(this.cacheMetadata.CacheHint == CacheHint.DistributedCache)
                                {
                                    // Set the expiry only for REDIS cache. For In-memory cache the values will be saved forever.
                                    var redisCacheTtlInSeconds = await this.cacheConfigurationHandler.GetRedisCacheTtlInSeconds();
                                    expiry = TimeSpan.FromSeconds(redisCacheTtlInSeconds);
                                }
                            }
                        }
                    }

                    var mergedKey = this.GetCacheKey(key, entityName);
                    return await this.cacheStrategy.InsertOrUpdateAsync(mergedKey, entity, expiry);
                }
                catch(Exception ex)
                {
                    this.logger.LogException(ex);
                    return false;
                }
            }

            return true;
        }

        private ICacheStrategy<T> GetCacheStrategyAsync(CacheMetadata cacheMetadata)
        {
            switch (cacheMetadata.CacheHint)
            {
                case CacheHint.DistributedCache:
                    var cacheConnectionString = AsyncHelper.RunSync(() => this.cacheConfigurationHandler.GetCacheConnectionStringAsync());
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

        private string GetCacheKey(string key, string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                return $"{typeof(T).FullName}|{key}";
            }

            return $"{entityName}|{key}";
        }
    }
}
