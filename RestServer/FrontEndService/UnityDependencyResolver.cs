using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService
{
    using System.Web.Http.Dependencies;

    using RestServer.Logging;
    using RestServer.ServerContext;

    using Unity;
    using Unity.Exceptions;

    public class UnityDependencyResolver : IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        ~UnityDependencyResolver()
        {
            this.Dispose(false);
        }

        private bool isDisposed = false;

        private IUnityContainer container;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch (ResolutionFailedException ex)
            {
                LoggerEventSource.Current.Exception(RestServiceContext.ServiceStartupTraceId, ex);
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException ex)
            {
                LoggerEventSource.Current.Exception(RestServiceContext.ServiceStartupTraceId, ex);
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var childContainer = this.container.CreateChildContainer();
            return new UnityDependencyResolver(childContainer);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.container != null)
                {
                    this.container.Dispose();
                    this.container = null;
                    this.isDisposed = true;
                }
            }
        }
    }
}
