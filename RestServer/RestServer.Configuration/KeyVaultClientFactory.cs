using RestServer.Configuration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using RestServer.Logging.Interfaces;
using RestServer.KeyStore.Interfaces;
using Hyak.Common.TransientFaultHandling;
using Hyak.Common;
using System.Net.Http;
using RestServer.KeyStore;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Services.AppAuthentication;

namespace RestServer.Configuration
{
    public class KeyVaultClientFactory : IKeyVaultClientFactory
    {
        private IEventLogger logger;
        private IKeyVaultContext keyVaultContext;
        private static KeyVaultClient keyVaultClient;

        public KeyVaultClientFactory(IEventLogger logger, IKeyVaultContext keyVaultContext)
        {
            this.logger = logger;
            this.keyVaultContext = keyVaultContext;
        }

        public KeyVaultClient GetKeyVaultClient()
        {
            if(!this.keyVaultContext.Initialized || null == this.keyVaultContext.Settings)
            {
                throw new ArgumentException("Key vault context has not been initialized.");
            }

            if(null == keyVaultClient)
            {
                this.logger.LogInformation($"Creating key vault client for {this.keyVaultContext.Settings.VaultAddress}");
                var retryStrategy = new ExponentialBackoff(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));
                var retryPolicy = new RetryPolicy(new RestTransientErrorDetectionStrategy(), retryStrategy);
                var httpRetryHandler = new RetryHandler(retryPolicy, new KeyVaultDelegatingHandler());
                httpRetryHandler.Retrying += HttpRetryHandler_Retrying;

                // If the key vault 
                if (this.keyVaultContext.Settings.IsManagedIdentityUsedForKeyVaultAccess)
                {
                    AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();
                    keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));
                }
                else
                {
                    keyVaultClient = new KeyVaultClient(this.GetAccessToken, new HttpClient(httpRetryHandler));
                }

                this.logger.LogInformation($"Key vault client for {this.keyVaultContext.Settings.VaultAddress} created successfully.");
            }

            return keyVaultClient;
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var keyVaultConfig = this.keyVaultContext.Settings;
            var credential = this.GetClientAssertionCertificate(keyVaultConfig);
            var context = new AuthenticationContext(authority, null);

            AuthenticationResult authenticationResult = await context.AcquireTokenAsync(resource, credential).ConfigureAwait(false);
            return authenticationResult.AccessToken;
        }

        private void HttpRetryHandler_Retrying(object sender, RetryingEventArgs e)
        {
            this.logger.LogWarning($"Retrying key valult request Attempt: {e.CurrentRetryCount}, delay : {e.Delay}, Last Exception : {e.LastException?.Message}");
        }

        private ClientAssertionCertificate GetClientAssertionCertificate(KeyVaultConfiguration keyVaultConfig)
        {
            var certificateThumbprint = keyVaultConfig.ClientCertificateThumbrpint;
            var localCertificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            localCertificateStore.Open(OpenFlags.ReadOnly);
            var certificates = localCertificateStore.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
            if (certificates.Count == 0)
            {
                throw new ArgumentException($"Not able to find certificate with thumbprint {certificateThumbprint}");
            }
            else if (certificates.Count > 1)
            {
                this.logger.LogWarning($"Found multiple certificates with thumbprint {certificateThumbprint}");
            }

            var certificate = certificates[0];
            return new ClientAssertionCertificate(keyVaultConfig.ClientAuthId, certificate);
        }
    }


    public class RestTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            // TODO: Handle below transient exceptions:
            // HttpStatusCode.RequestTimeout, // 408
            // HttpStatusCode.InternalServerError, // 500
            // HttpStatusCode.BadGateway, // 502
            // HttpStatusCode.ServiceUnavailable, // 503
            // HttpStatusCode.GatewayTimeout // 504

            return false;
        }
    }
}
