using RestServer.Cache;
using RestServer.Configuration.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public class ConfigurationHandler : IConfigurationHandler
    {
        private IEventLogger logger;

        private IConfigurationStoreFactory configurationStoreFactory;

        private static readonly ConcurrentDictionary<string, AsyncLock> KeyBasedConcurrentLock = new ConcurrentDictionary<string, AsyncLock>();

        private static readonly ConcurrentDictionary<string, Type> CacheKeyTypeMap = new ConcurrentDictionary<string, Type>();

        public ConfigurationHandler(IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory)
        {
            this.logger = logger;
            this.configurationStoreFactory = configurationStoreFactory;
        }

        public Task<string> GetConfiguration(string key)
        {
            return this.GetConfiguration<string>(key);
        }

        public async Task<T> GetConfiguration<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Configuration key is missing.");
            }

            var configuration = InMemoryCacheManager.Get<T>(key);
            if(null != configuration && !EqualityComparer<T>.Default.Equals(configuration, default(T)))
            {
                return configuration;
            }

            // When multiple instances try to fetch the same key prevent going to the backend store by placing a lock on each key.
            using(await KeyBasedConcurrentLock.GetOrAdd(key, new AsyncLock()).LockAsync().ConfigureAwait(false))
            {
                // Once the key has been released by the first thread fetching the value, subsequent threads can attempt to read it from configuration again.
                configuration = InMemoryCacheManager.Get<T>(key);
                if (null != configuration && !EqualityComparer<T>.Default.Equals(configuration, default(T)))
                {
                    return configuration;
                }

                var configStore = this.configurationStoreFactory.GetConfigurationStoreByKey(key);
                configuration = await configStore.GetAsync<T>(key).ConfigureAwait(false);

                InMemoryCacheManager.Add<T>(key, configuration);

                // Store the type assosicated with key data to cast it to a type during an update operation.
                CacheKeyTypeMap[key] = typeof(T);
            }

            return configuration;
        }

        public async Task<ConfigurationChangeMessage.Status> UpdateConfigKeyDataInCache(string key)
        {
            var status = ConfigurationChangeMessage.Status.Success;
            try
            {
                if (CacheKeyTypeMap.ContainsKey(key))
                {
                    var typeForKeyData = CacheKeyTypeMap[key];
                    var configStore = this.configurationStoreFactory.GetConfigurationStoreByKey(key);
                    var configurationData = await configStore.GetAsync(key).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(configurationData))
                    {
                        CacheKeyTypeMap.TryRemove(key, out typeForKeyData);
                    }
                    else
                    {
                        var parseResult = configurationData.ParseType(typeForKeyData);

                        if(null != parseResult.Item1)
                        {
                            // If the data is returned successfully, store it in-memory.
                            InMemoryCacheManager.Add(key, parseResult.Item1);
                        }
                        else if(null != parseResult.Item2)
                        {
                            status = ConfigurationChangeMessage.Status.Failed;
                            this.logger.LogException($"Exception while parsing the data for {typeForKeyData}.", parseResult.Item2);
                        }
                    }
                }
                else
                {
                    status = ConfigurationChangeMessage.Status.Skipped;
                }
            }
            catch(Exception ex)
            {
                status = ConfigurationChangeMessage.Status.Failed;
                this.logger.LogException($"Exception occurred while updating config key data in cache.", ex);
            }

            return status;
        }

        public async Task<ConfigurationChangeMessage> UpdateAllConfigsInCache()
        {
            var configChangeMessage = new ConfigurationChangeMessage();
            try
            {
                if (CacheKeyTypeMap.Keys.Any())
                {
                    // Clear the secrets first so that any non-secret that references a secret gets the latest value if the non-secret is cleared.
                    var secretKeys = CacheKeyTypeMap.Keys.Where(key => key.StartsWith(ConfigurationConstants.SecretConfigurationPrefix, StringComparison.OrdinalIgnoreCase));
                    foreach(var secretKey in secretKeys)
                    {
                        configChangeMessage.UpdatedConfigurationKeys[secretKey] = await this.UpdateConfigKeyDataInCache(secretKey).ConfigureAwait(false);
                    }

                    // Delete the non-secret key data
                    var nonSecretKeys = CacheKeyTypeMap.Keys.Where(key => !key.StartsWith(ConfigurationConstants.SecretConfigurationPrefix, StringComparison.OrdinalIgnoreCase));
                    foreach (var nonSecretKey in nonSecretKeys)
                    {
                        configChangeMessage.UpdatedConfigurationKeys[nonSecretKey] = await this.UpdateConfigKeyDataInCache(nonSecretKey).ConfigureAwait(false);
                    }
                }
            }
            catch(Exception ex)
            {
                this.logger.LogException("Exception occurred while updating all configs in cache.", ex);
            }

            return configChangeMessage;
        }
    }
}
