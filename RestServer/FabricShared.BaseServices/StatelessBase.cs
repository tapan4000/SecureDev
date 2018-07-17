using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace RestServer.FabricShared.BaseServices
{
    using RestServer.IoC;
    using RestServer.Logging.Interfaces;
    using RestServer.ServerContext;

    public abstract class StatelessBase : StatelessService
    {
        protected IEventLogger Logger;

        protected StatelessBase(string traceId, StatelessServiceContext serviceContext) : base(serviceContext)
        {
            var unityContainer = IoCUnityHelper.GetConfiguredContainer();
            var dependencyContainer = new UnityDependencyContainer(unityContainer);
            this.Logger = dependencyContainer.Resolve<IEventLogger>();
            RestServiceContext.InitializeContext(traceId, RestServiceHostType.ServiceFabric, serviceContext.ServiceName.AbsolutePath, serviceContext.NodeContext.NodeName, serviceContext.CodePackageActivationContext.ApplicationName, serviceContext.ServiceTypeName);
        }
    }
}
