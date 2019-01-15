using RestServer.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheConfigurationHandler
    {
        Task<string> GetCacheConnectionStringAsync();

        Task<bool> IsRedisCacheEnabled();

        Task<int> GetRedisCacheTtlInSeconds();

        Task<RetrySetting> GetCacheRetrySetting();
    }
}
