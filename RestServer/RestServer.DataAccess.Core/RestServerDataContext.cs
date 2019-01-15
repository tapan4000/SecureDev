using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RestServer.Configuration.Interfaces;
using RestServer.Core.Helpers;
using RestServer.Configuration;
using RestServer.IoC;
using RestServer.DataAccess.Configuration;
using RestServer.Entities.DataAccess;

namespace RestServer.DataAccess.Core
{
    [IoCRegistration(IoCLifetime.Hierarchical)]
    public class RestServerDataContext : DataContextBase
    {
        static RestServerDataContext()
        {
            // Set the database initializer to null as we do not need to create database, considering that the database has been created already. The default strategy for
            // code-first contexts is an instance of CreateDatabaseIfNotExist<TContext>.
            Database.SetInitializer<RestServerDataContext>(null);
        }

        public RestServerDataContext(IConfigurationHandler configurationHandler) : base(AsyncHelper.RunSync(() => configurationHandler.GetConfiguration(ConfigurationConstants.SqlConnectionString)))
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.AppenCommonConfiguration(modelBuilder);
            this.AppendUserRelatedConfiguration(modelBuilder);
            this.AppendGroupRelatedConfiguration(modelBuilder);
            this.AppendEmergencySessionRelatedConfiguration(modelBuilder);
            this.AppendDeviceRelatedConfiguration(modelBuilder);
            this.AppendLocationConfiguration(modelBuilder);
        }

        private void AppendUserRelatedConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserActivationConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserDeviceConfiguration());
            modelBuilder.Configurations.Add(new UserSessionConfiguration());
        }

        private void AppendGroupRelatedConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CityBasedPublicGroupConfiguration());
            modelBuilder.Configurations.Add(new CountryBasedPublicGroupConfiguration());
            modelBuilder.Configurations.Add(new GroupCategoryConfiguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Configurations.Add(new GroupDeviceConfiguration());
            modelBuilder.Configurations.Add(new GroupMemberConfiguration());
            modelBuilder.Configurations.Add(new LocalityBasedPublicGroupConfiguration());
            modelBuilder.Configurations.Add(new PublicGroupConfiguration());
            modelBuilder.Configurations.Add(new StateBasedPublicGroupConfiguration());
            modelBuilder.Configurations.Add(new AnonymousGroupMemberConfiguration());
        }

        private void AppendEmergencySessionRelatedConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new EmergencyLocationConfiguration());
            modelBuilder.Configurations.Add(new LocationCaptureSessionConfiguration());
            modelBuilder.Configurations.Add(new EmergencySessionExtensionConfiguration());
            modelBuilder.Configurations.Add(new EmergencySessionPublicGroupAccessConfiguration());
            modelBuilder.Configurations.Add(new EmergencySessionViewerConfiguration());
        }

        private void AppendDeviceRelatedConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DeviceConfiguration());
        }

        private void AppenCommonConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new RestServerSettingConfiguration());
            modelBuilder.Configurations.Add(new ApplicationConfiguration());
        }

        private void AppendLocationConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LocationCaptureSessionConfiguration());
        }
    }
}
