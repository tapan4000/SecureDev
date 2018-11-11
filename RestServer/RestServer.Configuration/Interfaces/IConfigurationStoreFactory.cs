using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Interfaces
{
    public interface IConfigurationStoreFactory
    {
        IConfigurationStore GetConfigurationStoreByKey(string key);
    }
}
