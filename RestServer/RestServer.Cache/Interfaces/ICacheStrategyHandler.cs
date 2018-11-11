using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheStrategyHandler<T>
    {
        Task<bool> DoesKeyExistInStoreAsync(string key, string keyGroupName = null);

        Task<T> GetFromStoreAsync(string key, string keyGroupName = null);

        Task<bool> InsertOrUpdateInStoreAsync(string key, T entity, string keyGroupName = null, TimeSpan? expiry = null);

        Task<bool> ClearStoreCacheAsync();
    }
}
