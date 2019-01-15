using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core
{
    public class EntityFrameworkDbConfiguration : DbConfiguration
    {
        private int sqlRetryCount;

        private int sqlRetryIntervalInSeconds;

        private IEventLogger logger;

        private const string ProviderName = "RestServer.EntityFramework";

        private const int DefaultRetryIntervalInSeconds = 2;

        private const int DefaultRetryCount = 3;

        public EntityFrameworkDbConfiguration()
        {
            // This default constructor is required for a class extending DbConfiguration.
            this.InitializeDbExecutionStrategy();
            this.SetDatabaseLogFormatter((context, writeAction) => new EntityFrameworkDatabaseLogFormatter(context, writeAction));
        }

        public EntityFrameworkDbConfiguration(int sqlRetryCount, int sqlRetryIntervalInSeconds, IEventLogger logger)
        {
            this.sqlRetryCount = sqlRetryCount;
            this.sqlRetryIntervalInSeconds = sqlRetryIntervalInSeconds;
            this.logger = logger;
            this.InitializeDbExecutionStrategy();
            this.SetDatabaseLogFormatter((context, writeAction) => new EntityFrameworkDatabaseLogFormatter(context, writeAction));
        }

        private void InitializeDbExecutionStrategy()
        {
            try
            {
                this.SetExecutionStrategy(ProviderName, () => 
                    new SqlAzureExecutionStrategy(this.sqlRetryCount >= 0 ? this.sqlRetryCount : DefaultRetryCount, 
                    TimeSpan.FromSeconds(this.sqlRetryIntervalInSeconds >= 0 ? this.sqlRetryIntervalInSeconds : DefaultRetryIntervalInSeconds)));
            }
            catch(Exception ex)
            {
                this.logger.LogException(ex);
            }
        }
    }
}
