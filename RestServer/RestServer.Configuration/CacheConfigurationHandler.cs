using RestServer.Cache;
using RestServer.Cache.Interfaces;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration.Models;
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

        public async Task<int> GetRedisCacheTtlInSeconds()
        {
            var redisCacheTtlInSeconds = await this.configurationHandler.GetConfiguration<int>(ConfigurationConstants.RedisCacheTtlInSeconds);
            if(redisCacheTtlInSeconds <= 0)
            {
                return -1;
            }

            return redisCacheTtlInSeconds;
        }

        public async Task<bool> IsRedisCacheEnabled()
        {
            var isRedisCacheEnabled = await this.configurationHandler.GetConfiguration<bool>(ConfigurationConstants.IsRedisCacheEnabled);
            return isRedisCacheEnabled;
        }
    }
}
