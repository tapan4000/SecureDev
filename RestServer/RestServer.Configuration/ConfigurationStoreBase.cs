using RestServer.Cache;
using RestServer.Configuration.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public abstract class ConfigurationStoreBase : IConfigurationStore
    {
        private readonly string keyIdentifier;

        protected IEventLogger Logger;

        private IConfigurationStoreFactory configurationStoreFactory;

        public ConfigurationStoreBase(string keyIdentifier, IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory)
        {
            this.keyIdentifier = keyIdentifier;
            this.Logger = logger;
            this.configurationStoreFactory = configurationStoreFactory;
        }

        public abstract ConfigurationStoreType StoreType { get; }

        public async Task<string> GetAsync(string key)
        {
            return await this.GetAsync<string>(key, true).ConfigureAwait(false);
        }
        
        public async Task<T> GetAsync<T>(string key)
        {
            return await this.GetAsync<T>(key, true).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string key, bool shouldCache)
        {
            if (shouldCache)
            {
                var config = InMemoryCacheManager.Get<T>(key);
                if(null != config && !EqualityComparer<T>.Default.Equals(config, default(T)))
                {
                    return config;
                }
            }

            var keyWithoutIdentifier = key.Replace(this.keyIdentifier, string.Empty);
            var configData = await this.DoGetFromStoreAsync(keyWithoutIdentifier).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(configData))
            {
                this.Logger.LogError($"The requested key {key} is not found in the key store.");
                return default(T);
            }

            var typedValue = default(T);
            var parseResult = configData.ParseType(typeof(T));

            if(null != parseResult.Item2)
            {
                this.Logger.LogException($"Error occurred while parsing item of type {typeof(T)} during key fetch for {key}.", parseResult.Item2);
                return typedValue;
            }

            if(null != parseResult.Item1)
            {
                typedValue = (T)parseResult.Item1;
            }

            return typedValue;
        }

        protected Task<T> GetConfigurationFromTargetStoreByKeyAync<T>(string key)
        {
            var targetStore = this.configurationStoreFactory.GetConfigurationStoreByKey(key);
            return targetStore.GetAsync<T>(key);
        }

        protected abstract Task<string> DoGetFromStoreAsync(string keyWithoutIdentifier);
    }
}
