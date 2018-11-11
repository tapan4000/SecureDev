using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Interfaces
{
    public interface IConfigurationStore
    {
        ConfigurationStoreType StoreType { get; }

        Task<string> GetAsync(string key);

        Task<T> GetAsync<T>(string key);

        Task<T> GetAsync<T>(string key, bool shouldCache);
    }
}
