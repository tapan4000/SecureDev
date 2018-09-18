using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FabricShared.BaseServices
{
    using System.Fabric;
    using System.Threading;

    using Microsoft.Owin.Hosting;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;

    using Owin;

    using RestServer.Logging.Interfaces;

    public sealed class OwinCommunicationListener : ICommunicationListener, IDisposable
    {
        private CancellationTokenSource serverCancellationTokenSource;

        private readonly ServiceContext serviceContext;

        private readonly string endpointName;

        private readonly Action<IAppBuilder> startupAction;

        private readonly IEventLogger logger;

        private readonly string urlSuffix;

        private IDisposable webApp;

        private StatelessServiceContext serviceContext1;
        private string name;
        private Action<IAppBuilder> configureHttp;

        public OwinCommunicationListener(ServiceContext serviceContext, string endpointName, Action<IAppBuilder> startupAction, IEventLogger logger, string urlSuffix)
        {
            if (null == serviceContext)
            {
                throw new ArgumentException(nameof(serviceContext));
            }

            if (null == endpointName)
            {
                throw new ArgumentException(nameof(endpointName));
            }

            if (null == startupAction)
            {
                throw new ArgumentException(nameof(startupAction));
            }

            if (null == logger)
            {
                throw new ArgumentException(nameof(logger));
            }

            this.serviceContext = serviceContext;
            this.endpointName = endpointName;
            this.startupAction = startupAction;
            this.urlSuffix = urlSuffix;
            this.logger = logger;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            this.serverCancellationTokenSource = new CancellationTokenSource();

            try
            {
                this.webApp = WebApp.Start(
                    this.BuildListeningAddress(),
                    appBuilder => this.startupAction.Invoke(appBuilder));

                var publishAddress = this.BuildPublishAddress();

                this.logger.LogInformation(ServiceFabricLogHelper.GetServiceContextAsString(this.serviceContext) + $"Web server started listening on {publishAddress}");
                return Task.FromResult(publishAddress);
            }
            catch (Exception ex)
            {
                this.logger.LogException($"Web server failed to open. Context: {ServiceFabricLogHelper.GetServiceContextAsString(this.serviceContext)}", ex);
                this.StopWebServer();
                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (null != this.serverCancellationTokenSource)
            {
                this.serverCancellationTokenSource.Dispose();
            }

            if (null != this.webApp)
            {
                this.webApp.Dispose();
            }
        }

        private string BuildPublishAddress()
        {
            return this.BuildAddress(FabricRuntime.GetNodeContext().IPAddressOrFQDN);
        }

        private string BuildListeningAddress()
        {
            return this.BuildAddress("+");
        }

        private void StopWebServer()
        {
            if (null != this.webApp)
            {
                try
                {
                    this.webApp.Dispose();
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private string BuildAddress(string hostName)
        {
            var serviceEndpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint(this.endpointName);
            var protocol = serviceEndpoint.Protocol;
            int port = serviceEndpoint.Port;
            var serviceUrlSuffix = this.urlSuffix;

            if (string.IsNullOrWhiteSpace(serviceUrlSuffix))
            {
                serviceUrlSuffix =
                    this.serviceContext.ServiceName.Segments[this.serviceContext.ServiceName.Segments.Length - 1];
            }

            return $"{protocol}://{hostName}:{port}/{serviceUrlSuffix}";
        }
    }
}
