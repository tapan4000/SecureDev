using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheStrategyHandler<T>
    {
        Task<bool> DoesKeyExistInStoreAsync(string key, string entityName = null);

        Task<T> GetFromStoreAsync(string key, string entityName = null);

        Task<bool> InsertOrUpdateInStoreAsync(string key, T entity, TimeSpan? expiry = null, string entityName = null);

        Task<bool> DeleteFromStoreAsync(string key, string entityName = null);

        Task<bool> ClearStoreCacheAsync();
    }
}
