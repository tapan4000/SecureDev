using Microsoft.Azure.KeyVault;
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

        private KeyVaultClient keyVaultClient;

        private IKeyVaultClientFactory keyVaultClientFactory;

        public SecretConfigurationStore(IKeyVaultContext keyVaultContext, IKeyVaultClientFactory keyVaultClientFactory, IDependencyContainer dependencyContainer, IEventLogger logger, IConfigurationStoreFactory configurationStoreFactory)
            :base(ConfigurationConstants.SecretConfigurationPrefix, logger, configurationStoreFactory)
        {
            this.keyVaultContext = keyVaultContext;
            this.keyVaultClientFactory = keyVaultClientFactory;
            this.dependencyContainer = dependencyContainer;
        }

        public override ConfigurationStoreType StoreType
        {
            get
            {
                return ConfigurationStoreType.Secret;
            }
        }

        protected async override Task<string> DoGetFromStoreAsync(string keyWithoutIdentifier)
        {
            await this.SetKeyVaultContextAsync().ConfigureAwait(false);

            if (null == this.keyVaultClient)
            {
                this.keyVaultClient = this.keyVaultClientFactory.GetKeyVaultClient();
            }
            var keyVaultUrl = this.keyVaultContext.Settings.VaultAddress;
            if (keyVaultUrl == null)
            {
                throw new Exception("Key Vault URL not found in cloud configuration!");
            }

            keyWithoutIdentifier = keyWithoutIdentifier.ToLower();
            var keyUri = string.Format(ConfigurationConstants.KeyVaultSecretUriFormat, keyVaultUrl, keyWithoutIdentifier);
            var secret = await this.keyVaultClient.GetSecretAsync(keyUri).ConfigureAwait(false);
            return null != secret ? secret.Value : string.Empty;
        }

        private async Task SetKeyVaultContextAsync()
        {
            if (!this.keyVaultContext.Initialized)
            {
                var keyVaultConfig = await BuildKeyVaultConfigurationAsync();
                this.keyVaultContext.InitializeSettings(keyVaultConfig);
            }
        }

        private async Task<KeyVaultConfiguration> BuildKeyVaultConfigurationAsync()
        {
            var keyVaultUrl = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultUrl).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                this.Logger.LogError("Key Vault URL cannot be empty.");
            }

            string keyVaultClientAuthId = null;
            string keyVaultClientCertificateThumbprint = null;

            var isManagedIdentityUsedForKeyVaultAccess = await this.GetConfigurationFromTargetStoreByKeyAync<bool>(ConfigurationConstants.IsManagedIdentityUsedForKeyVaultAccess).ConfigureAwait(false);
            if (!isManagedIdentityUsedForKeyVaultAccess)
            {
                keyVaultClientAuthId = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultClientAuthId).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(keyVaultClientAuthId))
                {
                    this.Logger.LogError("Key Vault Client Auth Id cannot be empty.");
                }

                keyVaultClientCertificateThumbprint = await this.GetConfigurationFromTargetStoreByKeyAync<string>(ConfigurationConstants.KeyVaultClientCertificateThumbprint).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(keyVaultClientCertificateThumbprint))
                {
                    this.Logger.LogError("Key Vault Client Certificate thumbprint cannot be empty.");
                }
            }

            var keyVaultCacheExpirationDurationInSeconds = await this.GetConfigurationFromTargetStoreByKeyAync<int>(ConfigurationConstants.KeyVaultCacheExpirationDurationInSeconds).ConfigureAwait(false);
            if (keyVaultCacheExpirationDurationInSeconds <= 0)
            {
                this.Logger.LogError("Key Vault cache expiration duration cannot be less than or equal to 0.");
            }

            var keyVaultConfiguration = new KeyVaultConfiguration
            {
                VaultAddress = keyVaultUrl,
                IsManagedIdentityUsedForKeyVaultAccess = isManagedIdentityUsedForKeyVaultAccess,
                ClientAuthId = keyVaultClientAuthId,
                ClientCertificateThumbrpint = keyVaultClientCertificateThumbprint,
                CacheExpirationDurationInSeconds = keyVaultCacheExpirationDurationInSeconds
            };

            return keyVaultConfiguration;
        }
    }
}
