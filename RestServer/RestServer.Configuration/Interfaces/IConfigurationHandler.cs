using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Interfaces
{
    public interface IConfigurationHandler
    {
        Task<T> GetConfiguration<T>(string key);

        Task<string> GetConfiguration(string key);
    }
}
