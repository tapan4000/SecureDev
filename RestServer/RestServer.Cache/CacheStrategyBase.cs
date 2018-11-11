using RestServer.Cache.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public abstract class CacheStrategyBase<T> : ICacheStrategy<T>
    {
        public abstract Task<bool> ClearCacheAsync();

        public abstract Task<bool> DeleteAsync(string key, string keyGroupName = null);

        public abstract Task<bool> DoesKeyExistAsync(string key, string keyGroupName = null);

        public abstract Task<T> GetAsync(string key, string keyGroupName = null);

        public abstract Task<bool> InsertOrUpdateAsync(string key, T entity, string keyGroupName = null, TimeSpan? expiry = default(TimeSpan?));

        protected string GetCacheKey(string key, string keyGroupName)
        {
            if (string.IsNullOrWhiteSpace(keyGroupName))
            {
                return $"{typeof(T).FullName}|{key}";
            }

            return $"{keyGroupName}|{key}";
        }
    }
}
