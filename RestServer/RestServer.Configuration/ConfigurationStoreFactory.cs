using RestServer.Configuration.Interfaces;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public class ConfigurationStoreFactory : IConfigurationStoreFactory
    {
        private IDependencyContainer dependencyContainer;

        public ConfigurationStoreFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IConfigurationStore GetConfigurationStoreByKey(string key)
        {
            var storeType = ConfigurationStoreType.Service;

            if(key.StartsWith(ConfigurationConstants.ServiceConfigurationPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                storeType = ConfigurationStoreType.Service;
            }

            if (key.StartsWith(ConfigurationConstants.SecretConfigurationPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                storeType = ConfigurationStoreType.Secret;
            }

            if (key.StartsWith(ConfigurationConstants.ExternalStorageConfigurationPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                storeType = ConfigurationStoreType.ExternalStorage;
            }

            var storeConvention = $"{storeType}ConfigurationStore";
            return this.dependencyContainer.Resolve<IConfigurationStore>(storeConvention);
        }
    }
}
