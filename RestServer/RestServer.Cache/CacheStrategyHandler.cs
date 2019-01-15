using RestServer.Cache.Core.Enums;
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

        private IConsolidatedCacheInvalidator consolidatedCacheInvalidator;

        private ICacheStrategy<T> cacheStrategy;

        private CacheMetadata cacheMetadata;

        public CacheStrategyHandler(IDependencyContainer dependencyContainer, 
            IEventLogger logger, 
            ICacheMetadataProvider cacheMetadataProvider, 
            ICacheConfigurationHandler cacheConfigurationHandler,
            IConsolidatedCacheInvalidator consolidatedCacheInvalidator)
        {
            this.cacheMetadata = cacheMetadataProvider.GetCacheMetadata(typeof(T));
            this.dependencyContainer = dependencyContainer;
            this.logger = logger;
            this.cacheConfigurationHandler = cacheConfigurationHandler;
            this.cacheStrategy = this.GetCacheStrategyAsync(this.cacheMetadata);
            this.consolidatedCacheInvalidator = consolidatedCacheInvalidator;
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

        public async Task<bool> DoesKeyExistInStoreAsync(string keyCategoryAndIdentifier, string entityName = null)
        {
            if(null != this.cacheStrategy && !keyCategoryAndIdentifier.IsEmpty())
            {
                try
                {
                    var mergedKey = CacheHelper.GetCacheKey<T>(keyCategoryAndIdentifier, entityName);
                    return await this.cacheStrategy.DoesKeyExistAsync(mergedKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            return false;
        }

        public async Task<T> GetFromStoreAsync(string keyCategoryAndIdentifier, string entityName = null)
        {
            if (null != this.cacheStrategy && !keyCategoryAndIdentifier.IsEmpty())
            {
                try
                {
                    var mergedKey = CacheHelper.GetCacheKey<T>(keyCategoryAndIdentifier, entityName);
                    return await this.cacheStrategy.GetAsync(mergedKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            return default(T);
        }

        public Task<bool> DeleteFromStoreAsync(string keyCategoryWithIdentifier, string entityName = null)
        {
            if (null != this.cacheStrategy && !keyCategoryWithIdentifier.IsEmpty())
            {
                try
                {
                    var mergedKey = CacheHelper.GetCacheKey<T>(keyCategoryWithIdentifier, entityName);
                    this.consolidatedCacheInvalidator.Register(this.cacheStrategy, mergedKey);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            // Returning true as deletion if key would fail if the cachestrategy is null or key is null (which can happen if the key is not already populated in the cache).
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteFromStoreAsync(IList<KeyValuePair<string, string>> keyCategoryIdentifierAndEntityNamePairs)
        {
            var result = true;
            foreach(var keyCategoryIdentifierAndEntityPair in keyCategoryIdentifierAndEntityNamePairs)
            {
                result = result && (await this.DeleteFromStoreAsync(keyCategoryIdentifierAndEntityPair.Key, keyCategoryIdentifierAndEntityPair.Value).ConfigureAwait(false));
            }

            return result;
        }

        public Task<bool> DeleteFromStoreAsync(IList<string> finalKeys)
        {
            if (null != this.cacheStrategy)
            {
                try
                {
                    this.consolidatedCacheInvalidator.Register(this.cacheStrategy, finalKeys);
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            }

            // Returning true as deletion if key would fail if the cachestrategy is null or key is null (which can happen if the key is not already populated in the cache).
            return Task.FromResult(true);
        }

        public async Task<bool> InsertOrUpdateInStoreAsync(string keyCategoryAndIdentifier, T entity, TimeSpan? expiry = null, string entityName = null)
        {
            if(null != this.cacheStrategy && !keyCategoryAndIdentifier.IsEmpty())
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

                    var mergedKey = CacheHelper.GetCacheKey<T>(keyCategoryAndIdentifier, entityName);
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
            return (int)cacheArea;
        }
    }
}
