using RestServer.Cache;
using RestServer.Cache.Interfaces;
using RestServer.Configuration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public class CacheConfigurationHandler : ICacheConfigurationHandler
    {
        private IConfigurationHandler configurationHandler;

        public CacheConfigurationHandler(IConfigurationHandler configurationHandler)
        {
            this.configurationHandler = configurationHandler;
        }

        public async Task<string> GetCacheConnectionStringAsync()
        {
            var cacheConnectionString = await this.configurationHandler.GetConfiguration(ConfigurationConstants.RedisCacheConnectionString);
            if (string.IsNullOrWhiteSpace(cacheConnectionString))
            {
                throw new NullReferenceException("Unable to fetch cache connection string.");
            }

            return cacheConnectionString;
        }

        public async Task<CacheConfigurationSettings> GetCacheConfigurationSettingsAsync()
        {
            var cacheConfigurationSettings = await this.configurationHandler.GetConfiguration<CacheConfigurationSettings>(ConfigurationConstants.CacheConfigurationSettings);
            if (null == cacheConfigurationSettings)
            {
                throw new NullReferenceException("Unable to fetch cache configuration settings.");
            }

            return cacheConfigurationSettings;
        }
    }
}
