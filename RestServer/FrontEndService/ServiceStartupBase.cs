namespace RestServer.FrontEndService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Web.Cors;
    using System.Web.Http;

    using Owin;
    using IoC;

    using Microsoft.Owin.Cors;

    using Newtonsoft.Json;

    using RestServer.FrontEndService.Converter;
    using RestServer.Logging.Interfaces;
    using Configuration.Interfaces;
    using Core.Helpers;
    using Configuration;
    using Cache;
    using Configuration.Models;

    public abstract class ServiceStartupBase
    {
        private readonly IDictionary<Type, object> explicitRegistrations;

        private IConfigurationHandler configurationHandler;

        private IEventLogger logger;

        public void ConfigureHttp(IAppBuilder appBuilder)
        {
            //var config = new HttpConfiguration();
            //this.ConfigureHttpRoutes(config);
            //appBuilder.UseWebApi(config);

            // Configure Web API for self-host 
            var config = new HttpConfiguration();

            // configure Unity
            var unityContainer = IoCUnityHelper.GetConfiguredContainer();
            IoCUnityHelper.OverrideDependencies(unityContainer, null, this.explicitRegistrations);
            var unityDependencyResolver = new UnityDependencyResolver(unityContainer);
            var dependencyContainer = new UnityDependencyContainer(unityContainer);
            config.DependencyResolver = unityDependencyResolver;
            this.configurationHandler = (IConfigurationHandler)unityDependencyResolver.GetService(typeof(IConfigurationHandler));

            // Configure Error Page
            this.ConfigureGlobalFilters(config);

            // Configure Message Handlers
            this.ConfigureMessageHandlers(config);

            this.logger = dependencyContainer.Resolve<IEventLogger>();

            AsyncHelper.RunSync(this.InitializeRedis);

            // Enable CORS
            this.AllowCors(appBuilder);

            // Configure Formatters
            config.Formatters.Clear();
            this.ConfigureFormatters(config);

            // Configure Type Converters
            this.ConfigureTypeConverters(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Configure Routes
            this.ConfigureHttpsRoutes(config);

            // Configure to use Web Api
            appBuilder.UseWebApi(config);

            // Post Process
            this.PostProcess(config);
        }

        public void ConfigureAppHttps(IAppBuilder appBuilder)
        {
            //// Configure Web API for self-host 
            //var config = new HttpConfiguration();

            //// configure Unity
            //var unityContainer = IoCUnityHelper.GetConfiguredContainer();
            //IoCUnityHelper.OverrideDependencies(unityContainer, null, this.explicitRegistrations);
            //var unityDependencyResolver = new UnityDependencyResolver(unityContainer);
            //var dependencyContainer = new UnityDependencyContainer(unityContainer);
            //config.DependencyResolver = unityDependencyResolver;

            //// Configure Error Page
            //this.ConfigureGlobalFilters(config);

            //// Configure Message Handlers
            //this.ConfigureMessageHandlers(config);

            //this.logger = dependencyContainer.Resolve<IEventLogger>();
            //AsyncHelper.RunSync(this.InitializeRedis);

            //// Enable CORS
            //this.AllowCors(appBuilder);

            //// Configure Formatters
            //config.Formatters.Clear();
            //this.ConfigureFormatters(config);

            //// Configure Type Converters
            //this.ConfigureTypeConverters(config);

            //// Web API routes
            //config.MapHttpAttributeRoutes();

            //// Configure Routes
            //this.ConfigureHttpsRoutes(config);

            //// Configure to use Web Api
            //appBuilder.UseWebApi(config);

            //// Post Process
            //this.PostProcess(config);
        }

        protected virtual void PostProcess(HttpConfiguration config)
        {
        }

        protected virtual void ConfigureMessageHandlers(HttpConfiguration config)
        {
        }

        protected virtual void ConfigureGlobalFilters(HttpConfiguration config)
        {
        }

        protected virtual void ConfigureHttpRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "EndpointMonitorApi",
                routeTemplate: "api/endpointmonitor",
                defaults: new { controller = "endpointmonitor" });
        }

        protected abstract void ConfigureHttpsRoutes(HttpConfiguration config);

        protected virtual void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.Add(new JsonMediaTypeFormatter());
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        protected virtual void ConfigureTypeConverters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new JsonSecureStringRequestModelConverter());
        }

        private async Task InitializeRedis()
        {
            var cacheConnectionString = await this.configurationHandler.GetConfiguration(ConfigurationConstants.RedisCacheConnectionString);
            var multiplexerPoolSize = await this.configurationHandler.GetConfiguration<int>(ConfigurationConstants.RedisCacheConnectionMultiplexerPoolSize);
            var globalSetting = await this.configurationHandler.GetConfiguration<GlobalSetting>(ConfigurationConstants.GlobalSetting);
            RedisConnectionMultiplexer.Instance.Initialize(new[] { cacheConnectionString }, globalSetting.MinIocpThreadCountForMaxRedisThroughput, this.logger, multiplexerPoolSize);
        }

        private void AllowCors(IAppBuilder appBuilder)
        {
            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = true,
                SupportsCredentials = true
            };

            policy.ExposedHeaders.Add("X-Custom-Header");

            appBuilder.UseCors(new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(policy)
                }
            });
        }
    }
}
