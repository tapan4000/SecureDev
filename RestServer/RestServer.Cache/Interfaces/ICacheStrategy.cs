using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheStrategy
    {
        Task<bool> DeleteAsync(string key, string keyGroupName = null);
    }

    public interface ICacheStrategy<T> : ICacheStrategy
    {
        Task<bool> DoesKeyExistAsync(string key, string keyGroupName = null);

        Task<T> GetAsync(string key, string keyGroupName = null);

        Task<bool> InsertOrUpdateAsync(string key, T entity, string keyGroupName = null, TimeSpan? expiry = null);

        Task<bool> ClearCacheAsync();
    }
}
