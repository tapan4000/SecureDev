using System;

namespace RestServer.FabricShared.Configuration.Interfaces
{
    public interface IServiceConfigurationHandler
    {
        event EventHandler ConfigurationChangedEvent;

        string GetConfigurationValue(string key);
    }
}
