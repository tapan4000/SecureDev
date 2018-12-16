namespace RestServer.FrontEndService
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Tracing;
    using System.Threading;

    using Microsoft.ServiceFabric.Services.Runtime;

    using RestServer.FabricShared.BaseServices;
    using RestServer.Logging;
    using RestServer.ServerContext;

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
                    serviceContext =>
                        {
                            string dataCenter = "eus"; // TODO: Fetch this data from key vault.
                            int maxLogLevel = (int)EventLevel.Verbose; // Fetch service level setting and flow specific setting. Stored in data store.
                            bool isExecutionTimeLoggingEnabled = true; // Fetch global setting and flow specific setting. Stored in data store
                            RestServiceContext.InitializeContext(traceId, RestServiceHostType.ServiceFabric, serviceContext.ServiceName.AbsolutePath, serviceContext.NodeContext.NodeName, serviceContext.CodePackageActivationContext.ApplicationName, serviceContext.ServiceTypeName, dataCenter, maxLogLevel, isExecutionTimeLoggingEnabled);
                            return new FrontEndService(traceId, serviceContext);
                        }).GetAwaiter().GetResult();

                //LoggerEventSource.Current.Info(traceId, ServiceFabricLogHelper.GetServiceRegisteredMessage(Process.GetCurrentProcess().Id, typeof(FrontEndService).Name), null);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                LoggerEventSource.Current.Critical(traceId, e, 0);
                throw;
            }
        }
    }
}
