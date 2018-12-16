using RestServer.FabricShared.Configuration.Interfaces;
using RestServer.Configuration;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FabricShared.Configuration
{
    public class ServiceConfigurationHandler : IServiceConfigurationHandler
    {
        private const string ConfigurationPackageObjectName = "Config";

        private bool isConfigInitialized;

        private IEventLogger logger;

        private Uri serviceUri;

        private string serviceName;

        private Dictionary<string, string> serviceFabricConfiguration;

        private const string KeyVaultUrlKeyName = "KeyVaultUrl";

        private const string KeyVaultCertificateThumbprintKeyName = "KeyVaultCertificateThumbprint";

        private const string KeyVaultCacheExpirationDurationInSeconds = "KeyVaultCacheExpirationDurationInSeconds";

        public event EventHandler ConfigurationChangedEvent;

        public ServiceConfigurationHandler(string serviceStartupTraceId, Uri serviceUri, ICodePackageActivationContext context, IEventLogger logger)
        {
            if (null == serviceStartupTraceId)
            {
                throw new ArgumentException(nameof(serviceStartupTraceId));
            }

            if (null == serviceUri)
            {
                throw new ArgumentException(nameof(serviceUri));
            }

            if (null == context)
            {
                throw new ArgumentException(nameof(context));
            }

            if (null == logger)
            {
                throw new ArgumentException(nameof(logger));
            }

            this.logger = logger;
            this.serviceUri = serviceUri;
            this.serviceName = this.serviceUri.Segments[serviceUri.Segments.Length - 1];
            this.serviceFabricConfiguration = new Dictionary<string, string>();

            this.logger.LogInformation($"Initializing service configuration handler for service {this.serviceName}.");

            context.ConfigurationPackageAddedEvent += ConfigurationPackageAddedEvent;
            context.ConfigurationPackageModifiedEvent += ConfigurationPackageModifiedEvent;
            context.ConfigurationPackageRemovedEvent += ConfigurationPackageRemovedEvent;

            // Call the Configuration package added event in case of first initialization of the handler, so that the configuration is accessible immidiately.
            var packageAddedEvent = new PackageAddedEventArgs<ConfigurationPackage>
            {
                Package = context.GetConfigurationPackageObject(ConfigurationPackageObjectName)
            };

            this.ConfigurationPackageAddedEvent(null, packageAddedEvent);
            this.logger.LogInformation($"Service configuration handler initialization complete for service {this.serviceName}.");
        }

        /// <summary>
        /// Package remove event is triggered during an application upgrade.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PackageRemovedEventArgs{ConfigurationPackage}"/> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">e</exception>
        private void ConfigurationPackageRemovedEvent(object sender, PackageRemovedEventArgs<ConfigurationPackage> e)
        {
            this.logger.LogInformation("Package remove event triggered.");

            if (null == e)
            {
                this.logger.LogError("The argument for removed package event cannot be null");
                throw new ArgumentException(nameof(e));
            }
        }

        private void ConfigurationPackageModifiedEvent(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            this.logger.LogInformation("Package modified event triggered.");

            if (null == e)
            {
                this.logger.LogError("The argument for modified package event cannot be null");
                throw new ArgumentException(nameof(e));
            }

            this.LoadConfiguration(e.NewPackage.Settings.Sections);
        }

        private void ConfigurationPackageAddedEvent(object sender, PackageAddedEventArgs<ConfigurationPackage> e)
        {
            this.logger.LogInformation("Package added event triggered.");
            if (null == e)
            {
                this.logger.LogError("The argument for added package event cannot be null");
                throw new ArgumentException(nameof(e));
            }

            this.LoadConfiguration(e.Package.Settings.Sections);
        }

        public Dictionary<string, string> ServiceFabricConfiguration
        {
            get
            {
                if (this.isConfigInitialized)
                {
                    return this.serviceFabricConfiguration;
                }

                return null;
            }
        }

        private void LoadConfiguration(KeyedCollection<string, ConfigurationSection> sections)
        {
            this.isConfigInitialized = false;
            if (sections.Contains(ConfigurationConstants.ServiceConfigurationSectionName))
            {
                this.logger.LogInformation($"Configuration found for service {this.serviceUri.AbsoluteUri}");

                var section = sections[ConfigurationConstants.ServiceConfigurationSectionName];

                foreach(var param in section.Parameters)
                {
                    this.serviceFabricConfiguration.Add(param.Name, param.Value);
                }

                this.isConfigInitialized = true;
            }
        }

        public string GetConfigurationValue(string key)
        {
            if (!this.isConfigInitialized)
            {
                this.logger.LogError($"Config not yet initialized for fetching the key {key}.");
                return null;
            }

            var configValue = this.serviceFabricConfiguration[key];
            if (null == configValue)
            {
                this.logger.LogError($"Config value not found for key {key}.");
                return null;
            }

            return configValue;
        }
    }
}
