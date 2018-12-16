using RestServer.Cache.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class InMemoryCacheStrategy<T> : CacheStrategyBase<T>
    {
        public override Task<bool> ClearCacheAsync()
        {
            InMemoryCacheManager.ClearCache();
            return Task.FromResult(true);
        }

        public override Task<bool> DeleteAsync(string key)
        {
            InMemoryCacheManager.Remove(key);
            return Task.FromResult(true);
        }

        public override Task<bool> DoesKeyExistAsync(string key)
        {
            return Task.FromResult(InMemoryCacheManager.ContainsKey(key));
        }

        public override Task<T> GetAsync(string key)
        {
            return Task.FromResult(InMemoryCacheManager.Get<T>(key));
        }

        public override Task<bool> InsertOrUpdateAsync(string key, T entity, TimeSpan? expiry = null)
        {
            var expiryInSeconds = null == expiry ? -1 : Convert.ToInt32(expiry.Value.TotalSeconds);
            InMemoryCacheManager.Add<T>(key, entity, expiryInSeconds);
            return Task.FromResult(true);
        }
    }
}
