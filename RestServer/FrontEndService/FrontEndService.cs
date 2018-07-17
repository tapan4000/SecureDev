﻿namespace RestServer.FrontEndService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using RestServer.FabricShared.BaseServices;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    using RestServer.Core.Workflow;
    using RestServer.Logging;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class FrontEndService : StatelessBase
    {
        public FrontEndService(string traceId, StatelessServiceContext context)
            : base(traceId, context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var endpoints = new List<EndpointResourceDescription>();
            endpoints.AddRange(this.Context.CodePackageActivationContext.GetEndpoints());
            var startup = new FrontEndServiceStartup();
            return endpoints.Select(
                endpoint =>
                    {
                        return endpoint.Protocol == EndpointProtocol.Http
                        ? new ServiceInstanceListener(serviceContext =>
                            new OwinCommunicationListener(serviceContext, endpoint.Name, startup.ConfigureHttp,
                                this.Logger, null),
                                endpoint.Name)
                        : new ServiceInstanceListener(serviceContext =>
                            new OwinCommunicationListener(serviceContext, endpoint.Name, startup.ConfigureAppHttps,
                                this.Logger, null),
                                endpoint.Name);
                    });
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
