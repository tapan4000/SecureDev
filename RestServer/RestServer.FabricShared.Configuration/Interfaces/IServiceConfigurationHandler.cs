namespace RestServer.FabricShared.Configuration.Interfaces
{
    public interface IServiceConfigurationHandler
    {
        string GetConfigurationValue(string key);
    }
}
