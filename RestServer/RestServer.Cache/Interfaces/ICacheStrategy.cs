using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheStrategy
    {
        Task<bool> DeleteAsync(string key);
    }

    public interface ICacheStrategy<T> : ICacheStrategy
    {
        Task<bool> DoesKeyExistAsync(string key);

        Task<T> GetAsync(string key);

        Task<bool> InsertOrUpdateAsync(string key, T entity, TimeSpan? expiry = null);

        Task<bool> ClearCacheAsync();
    }
}
