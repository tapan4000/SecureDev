using RestServer.Configuration;
using RestServer.Configuration.Interfaces;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess
{
    public class ExternalStorageConfigurationStore : ConfigurationStoreBase
    {
        private IUnitOfWorkFactory unitOfWorkFactory;

        public ExternalStorageConfigurationStore(IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory, IUnitOfWorkFactory unitOfWorkFactory)
            : base(ConfigurationConstants.ExternalStorageConfigurationPrefix, logger, configurationStoreFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public override ConfigurationStoreType StoreType
        {
            get
            {
                return ConfigurationStoreType.ExternalStorage;
            }
        }

        protected async override Task<string> DoGetFromStoreAsync(string keyWithoutIdentifier)
        {
            using(var unitOfWork = this.unitOfWorkFactory.RestServerUnitOfWork)
            {
                var settingValue = await unitOfWork.RestServerSettingRepository.GetById(keyWithoutIdentifier).ConfigureAwait(false);
                if(null != settingValue)
                {
                    return settingValue.Value;
                }
            }

            return null;
        }
    }
}
