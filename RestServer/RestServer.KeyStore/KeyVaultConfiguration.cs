using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.KeyStore
{
    public class KeyVaultConfiguration : ICloneable
    {
        public string VaultAddress { get; set; }

        public string KeyName { get; set; }

        public string KeyIdentifier { get; set; }

        public string AlgorithmName { get; set; }

        public string ClientAuthId { get; set; }

        public string ClientAuthSecret { get; set; }

        public string ClientCertificateThumbrpint { get; set; }

        public int CacheExpirationDurationInSeconds { get; set; }

        public bool IsManagedIdentityUsedForKeyVaultAccess { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
