using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.KeyStore.Interfaces
{
    public interface IKeyVaultContext
    {
        bool Initialized { get; }

        KeyVaultConfiguration Settings { get; }

        void InitializeSettings(KeyVaultConfiguration keyVaultConfiguration);
    }
}
