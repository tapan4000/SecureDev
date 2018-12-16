using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace RestServer.FabricShared.BaseServices
{
    using System.Diagnostics.Tracing;

    using RestServer.IoC;
    using RestServer.Logging.Interfaces;
    using RestServer.ServerContext;
    using Configuration.Interfaces;
    using System.Threading;
    using Configuration;

    public abstract class StatelessBase : StatelessService
    {
        protected IEventLogger Logger;

        private UnityDependencyContainer dependencyContainer;

        private IServiceConfigurationHandler serviceConfigurationHandler;

        private string startupTraceId;

        private StatelessServiceContext serviceContext;

        protected StatelessBase(string traceId, StatelessServiceContext serviceContext) : base(serviceContext)
        {
            var unityContainer = IoCUnityHelper.GetConfiguredContainer();
            this.dependencyContainer = new UnityDependencyContainer(unityContainer);
            this.Logger = this.dependencyContainer.Resolve<IEventLogger>();
            this.startupTraceId = traceId;
            this.serviceContext = serviceContext;
            this.SetupAndRegisterServiceConfiguationHandler();
        }

        protected override Task OnCloseAsync(CancellationToken cancellationToken)
        {
            this.serviceConfigurationHandler.ConfigurationChangedEvent -= ServiceConfigurationHandler_ConfigurationChangedEvent;
            return this.CloseAsync(this.startupTraceId, cancellationToken);
        }

        protected virtual Task CloseAsync(string traceId, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        private void ServiceConfigurationHandler_ConfigurationChangedEvent(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetupAndRegisterServiceConfiguationHandler()
        {
            this.serviceConfigurationHandler = new ServiceConfigurationHandler(this.startupTraceId, this.serviceContext.ServiceName, this.serviceContext.CodePackageActivationContext, this.Logger);
            this.serviceConfigurationHandler.ConfigurationChangedEvent += ServiceConfigurationHandler_ConfigurationChangedEvent;
            this.dependencyContainer.RegisterInstance(this.serviceConfigurationHandler);
        }
    }
}
