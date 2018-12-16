using RestServer.IoC;
using RestServer.KeyStore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.KeyStore
{
    [IoCRegistration(IoCLifetime.Hierarchical)]
    public class KeyVaultContext : IKeyVaultContext
    {
        public KeyVaultContext()
        {
        }

        public bool Initialized { get; private set; }

        public KeyVaultConfiguration Settings
        {
            get; private set;
        }

        public void InitializeSettings(KeyVaultConfiguration keyVaultConfiguration)
        {
            this.Settings = keyVaultConfiguration;
            this.Initialized = true;
        }
    }
}
