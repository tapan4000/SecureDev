using RestServer.Cache;
using RestServer.Configuration.Interfaces;
using RestServer.IoC.Interfaces;
using RestServer.KeyStore;
using RestServer.KeyStore.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public class SecretConfigurationStore : ConfigurationStoreBase
    {
        private IKeyVaultContext keyVaultContext;

        private IDependencyContainer dependencyContainer;

        private IEventLogger logger;

        public SecretConfigurationStore(IKeyVaultContext keyVaultContext, IDependencyContainer dependencyContainer, IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory)
            :base(ConfigurationConstants.SecretConfigurationPrefix, logger, configurationStoreFactory)
        {
            this.keyVaultContext = keyVaultContext;
            this.dependencyContainer = dependencyContainer;
            this.logger = logger;
        }

        public override ConfigurationStoreType StoreType
        {
            get
            {
                return ConfigurationStoreType.Secret;
            }
        }

        protected override Task<string> DoGetFromStoreAsync(string keyWithoutIdentifier)
        {
            throw new NotImplementedException();
        }

        private async Task SetKeyVaultContextAsync()
        {
            var keyVaultConfig = InMemoryCacheManager.Get<KeyVaultConfiguration>(ConfigurationConstants.KeyVaultConfig);

            if(null == keyVaultConfig)
            {
                keyVaultConfig = await BuildKeyVaultConfigurationAsync();
            }

            this.keyVaultContext.InitializeSettings(keyVaultConfig);
        }

        private async Task<KeyVaultConfiguration> BuildKeyVaultConfigurationAsync()
        {
            var keyVaultUrl = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultUrl).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                this.logger.LogError("Key Vault URL cannot be empty.");
            }

            var keyVaultClientAuthId = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultClientAuthId).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(keyVaultClientAuthId))
            {
                this.logger.LogError("Key Vault Client Auth Id cannot be empty.");
            }

            var keyVaultClientCertificateThumbprint = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultClientCertificateThumbprint).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(keyVaultClientCertificateThumbprint))
            {
                this.logger.LogError("Key Vault Client Certificate thumbprint cannot be empty.");
            }

            var keyVaultCacheExpirationDurationInSeconds = await this.GetConfigurationFromTargetStoreByKeyAync<int>(ConfigurationConstants.KeyVaultCacheExpirationDurationInSeconds).ConfigureAwait(false);
            if (keyVaultCacheExpirationDurationInSeconds <= 0)
            {
                this.logger.LogError("Key Vault cache expiration duration cannot be less than or equal to 0.");
            }

            var keyVaultConfiguration = new KeyVaultConfiguration
            {
                VaultAddress = keyVaultUrl,
                ClientAuthId = keyVaultClientAuthId,
                ClientAuthSecret = keyVaultClientCertificateThumbprint,
                CacheExpirationDurationInSeconds = keyVaultCacheExpirationDurationInSeconds
            };

            return keyVaultConfiguration;
        }
    }
}
