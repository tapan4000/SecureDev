using RestServer.Configuration.Interfaces;
using System;
using System.Threading.Tasks;
using RestServer.Configuration;
using RestServer.FabricShared.Configuration.Interfaces;
using RestServer.Logging.Interfaces;

namespace RestServer.FabricShared.Configuration
{
    public class ServiceConfigurationStore : ConfigurationStoreBase
    {
        private IServiceConfigurationHandler serviceConfigurationHandler;

        private IEventLogger logger;

        public ServiceConfigurationStore(IServiceConfigurationHandler serviceConfigurationHandler, IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory) : 
            base(ConfigurationConstants.ServiceConfigurationPrefix, logger, configurationStoreFactory)
        {
            this.serviceConfigurationHandler = serviceConfigurationHandler;
            this.logger = logger;
        }

        public override ConfigurationStoreType StoreType
        {
            get
            {
                return ConfigurationStoreType.Service;
            }
        }

        protected override Task<string> DoGetFromStoreAsync(string keyWithoutIdentifier)
        {
            if (string.IsNullOrWhiteSpace(keyWithoutIdentifier))
            {
                throw new ArgumentException(nameof(keyWithoutIdentifier));
            }

            var configValue = this.serviceConfigurationHandler.GetConfigurationValue(keyWithoutIdentifier); ;

            return Task.FromResult(configValue);
        }
    }
}
