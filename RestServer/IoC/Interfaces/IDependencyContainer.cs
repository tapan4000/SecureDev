namespace RestServer.IoC.Interfaces
{
    using System;

    public interface IDependencyContainer : IDisposable
    {
        bool IsRegistered<T>(string name);

        void RegisterInstance<T>(T instance);

        T Resolve<T>();

        T Resolve<T>(string name);

        T Resolve<T>(params DependencyParameterOverride[] parameterOverrides);

        void RegisterType<T>(Type target, string name);

        IDependencyContainer CreateChildContainer();
    }
}
