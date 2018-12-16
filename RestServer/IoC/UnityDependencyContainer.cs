namespace RestServer.IoC
{
    using System;

    using RestServer.IoC.Interfaces;

    using Unity;
    using Unity.Lifetime;
    using Unity.Resolution;
    using System.Collections.Generic;

    public class UnityDependencyContainer : IDependencyContainer
    {
        private IUnityContainer unityContainer;

        public UnityDependencyContainer(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public bool IsRegistered<T>(string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<T>(T instance)
        {
            this.unityContainer.RegisterInstance(instance);
        }

        public T Resolve<T>()
        {
            return this.unityContainer.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            name = name.ToLower();
            return this.unityContainer.Resolve<T>(name);
        }

        public T Resolve<T>(params DependencyParameterOverride[] parameterOverrides)
        {
            var parameters = new List<ResolverOverride>();

            foreach(var parameterOverride in parameterOverrides)
            {
                parameters.Add(new ParameterOverride(parameterOverride.ParameterName, parameterOverride.ParameterValue));
            }

            return this.unityContainer.Resolve<T>(parameters.ToArray());
        }

        public void RegisterType<T>(Type target, string name)
        {
            this.unityContainer.RegisterType(typeof(T), target, "ss", new SingletonLifetimeManager(), null);
        }

        public IDependencyContainer CreateChildContainer()
        {
            var childContainer = this.unityContainer.CreateChildContainer();
            return new UnityDependencyContainer(childContainer);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if(null != this.unityContainer)
                {
                    this.unityContainer.Dispose();
                }
            }
        }
    }
}
