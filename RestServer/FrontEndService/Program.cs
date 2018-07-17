namespace RestServer.FrontEndService
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.ServiceFabric.Services.Runtime;

    using RestServer.FabricShared.BaseServices;
    using RestServer.Logging;

    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            string traceId = Guid.NewGuid().ToString();
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                
                ServiceRuntime.RegisterServiceAsync("FrontEndServiceType",
                    context => new FrontEndService(traceId, context)).GetAwaiter().GetResult();
                LoggerEventSource.Current.Info(traceId, ServiceFabricLogHelper.GetServiceRegisteredMessage(Process.GetCurrentProcess().Id, typeof(FrontEndService).Name));

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                LoggerEventSource.Current.Exception(traceId, e);
                throw;
            }
        }
    }
}
