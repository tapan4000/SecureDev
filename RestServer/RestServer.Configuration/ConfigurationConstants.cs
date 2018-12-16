using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public class ConfigurationConstants
    {
        public const string ServiceConfigurationPrefix = "Service_";

        public const string ExternalStorageConfigurationPrefix = "External_";

        public const string SecretConfigurationPrefix = "Secret_";

        public const string RedisCacheConnectionString = SecretConfigurationPrefix + "RedisCacheConnectionString";

        public const string ServiceConfigurationSettings = ServiceConfigurationPrefix + "ServiceConfigurationSettings";

        // A configuration constant that does not have a prefix is not stored in any configuration store, rather it is built dynamically in memory.
        public const string KeyVaultConfig = "KeyVaultConfig";

        public const string KeyVaultUrl = ServiceConfigurationPrefix + "KeyVaultUrl";

        public const string KeyVaultClientAuthId = ServiceConfigurationPrefix + "KeyVaultClientAuthId";

        public const string KeyVaultClientCertificateThumbprint = ServiceConfigurationPrefix + "KeyVaultClientCertificateThumbprint";

        public const string KeyVaultCacheExpirationDurationInSeconds = ServiceConfigurationPrefix + "KeyVaultCacheExpirationDurationInSeconds";

        public const string LogLevel = ServiceConfigurationPrefix + "LogLevel";

        public const string SqlConnectionString = SecretConfigurationPrefix + "SqlConnectionString";

        public const string ServiceConfigurationSectionName = "ServiceConfig";

        public const string KeyVaultSecretUriFormat = "{0}/secrets/{1}";

        public const string TextLocalSmsApiKey = SecretConfigurationPrefix + "TextLocalSmsApiKey";

        public const string UserActivationSetting = ExternalStorageConfigurationPrefix + "UserActivationSetting";

        public const string SendGridApiKey = SecretConfigurationPrefix + "SendGridApiKey";

        public const string UserAuthTokenSigningKey = SecretConfigurationPrefix + "UserAuthTokenSigningKey";

        public const string UserAuthSetting = ExternalStorageConfigurationPrefix + "UserAuthSetting";

        public const string RedisCacheConnectionMultiplexerPoolSize = ServiceConfigurationPrefix + "RedisCacheConnectionMultiplexerPoolSize";

        public const string GlobalSetting = ExternalStorageConfigurationPrefix + "GlobalSetting";

        public const string IsRedisCacheEnabled = ServiceConfigurationPrefix + "IsRedisCacheEnabled";

        public const string RedisCacheTtlInSeconds = ServiceConfigurationPrefix + "RedisCacheTtlInSeconds";

        public const string GroupGeneralSetting = ExternalStorageConfigurationPrefix + "GroupGeneralSetting";
    }
}
