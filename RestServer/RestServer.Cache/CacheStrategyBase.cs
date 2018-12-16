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

        public abstract Task<bool> DeleteAsync(string key);

        public abstract Task<bool> DoesKeyExistAsync(string key);

        public abstract Task<T> GetAsync(string key);

        public abstract Task<bool> InsertOrUpdateAsync(string key, T entity, TimeSpan? expiry = null);
    }
}
