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

        public override Task<bool> DeleteAsync(string key, string keyGroupName = null)
        {
            InMemoryCacheManager.Remove(key);
            return Task.FromResult(true);
        }

        public override Task<bool> DoesKeyExistAsync(string key, string keyGroupName = null)
        {
            return Task.FromResult(InMemoryCacheManager.ContainsKey(key));
        }

        public override Task<T> GetAsync(string key, string keyGroupName = null)
        {
            return Task.FromResult(InMemoryCacheManager.Get<T>(key));
        }

        public override Task<bool> InsertOrUpdateAsync(string key, T entity, string keyGroupName = null, TimeSpan? expiry = null)
        {
            InMemoryCacheManager.Add<T>(key, entity);
            return Task.FromResult(true);
        }
    }
}
