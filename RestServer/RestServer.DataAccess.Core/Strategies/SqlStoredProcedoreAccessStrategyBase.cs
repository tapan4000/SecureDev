using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using RestServer.Cache.Interfaces;
using RestServer.Configuration;
using RestServer.Configuration.Interfaces;
using RestServer.Configuration.Models;
using RestServer.Core.Extensions;
using RestServer.Core.Helpers;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Strategies
{
    public abstract class SqlStoredProcedureAccessStrategyBase
    {
        private const int DefaultSqlRetryCount = 2;

        private const int DefaultSqlRetryIntervalInSeconds = 2;

        private const int DefaultSqlCommandTimeoutInSeconds = 30;

        private IConfigurationHandler configurationHandler;

        protected IEventLogger Logger;

        private IConsolidatedCacheInvalidator cacheInvalidator;

        private string sqlConnectionString;

        private GlobalSetting globalSettings;

        public SqlStoredProcedureAccessStrategyBase(IConfigurationHandler configurationHandler, IEventLogger logger, IConsolidatedCacheInvalidator cacheInvalidator)
        {
            this.configurationHandler = configurationHandler;
            this.Logger = logger;
            this.cacheInvalidator = cacheInvalidator;
            this.sqlConnectionString = AsyncHelper.RunSync(() => configurationHandler.GetConfiguration(ConfigurationConstants.SqlConnectionString));
            this.globalSettings = AsyncHelper.RunSync(() => this.configurationHandler.GetConfiguration<GlobalSetting>(ConfigurationConstants.GlobalSetting));
        }

        protected async Task<T> ExecuteProcedure<T>(string procedureName, Dictionary<string, string> parameters)
        {
            var storedProcedureExecutionRetryPolicy = this.GetRetryPolicy();

            T spResult;
            using (var connection = new ReliableSqlConnection(this.sqlConnectionString, storedProcedureExecutionRetryPolicy, storedProcedureExecutionRetryPolicy))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    command.CommandTimeout = this.globalSettings.SqlCommandTimeoutInSeconds > 0 ? this.globalSettings.SqlCommandTimeoutInSeconds : DefaultSqlCommandTimeoutInSeconds;
                    if (null != parameters)
                    {
                        foreach (var spParameterKey in parameters.Keys)
                        {
                            if (parameters[spParameterKey].IsEmpty())
                            {
                                command.Parameters.AddWithValue(spParameterKey, DBNull.Value);
                            }
                            else
                            {
                                command.Parameters.AddWithValue(spParameterKey, parameters[spParameterKey]);
                            }
                        }
                    }

                    spResult = connection.ExecuteCommand<T>(command);
                }
            }

            // Invalidate the cache post execution of the SP.
            await this.cacheInvalidator.invalidateAsync().ConfigureAwait(false);
            storedProcedureExecutionRetryPolicy = null;
            return spResult;
        }

        protected async Task<DataTable> ExecuteProcedureReturningTable(string procedureName, Dictionary<string, string> parameters)
        {
            var storedProcedureExecutionRetryPolicy = this.GetRetryPolicy();

            var spResultSet = new DataTable();
            using (var connection = new ReliableSqlConnection(this.sqlConnectionString, storedProcedureExecutionRetryPolicy, storedProcedureExecutionRetryPolicy))
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procedureName;
                command.CommandTimeout = this.globalSettings.SqlCommandTimeoutInSeconds > 0 ? this.globalSettings.SqlCommandTimeoutInSeconds : DefaultSqlCommandTimeoutInSeconds;
                if (null != parameters)
                {
                    foreach (var spParameterKey in parameters.Keys)
                    {
                        if (parameters[spParameterKey].IsEmpty())
                        {
                            command.Parameters.AddWithValue(spParameterKey, DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue(spParameterKey, parameters[spParameterKey]);
                        }
                    }
                }

                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    // Opening the connection to ensure the reliability mechanism is utilized
                    connection.Open();
                    storedProcedureExecutionRetryPolicy.ExecuteAction(
                        () =>
                        {
                            dataAdapter.Fill(spResultSet);
                        });
                }
            }

            // Invalidate the cache post execution of the SP.
            await this.cacheInvalidator.invalidateAsync().ConfigureAwait(false);
            storedProcedureExecutionRetryPolicy = null;
            return spResultSet;
        }

        private void StoredProcedureExecutionRetryPolicy_Retrying(object sender, RetryingEventArgs e)
        {
            this.Logger.LogWarning($"Transient error occured in SQL SP execution. Retrying attempt {e.CurrentRetryCount} after delay of {e.Delay.TotalSeconds} seconds. Last exception: {e.LastException.ToString()}");
        }

        private RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy> GetRetryPolicy()
        {
            var retryIntervalInSeconds = this.globalSettings.SqlRetryIntervalInSeconds > 0 ? this.globalSettings.SqlRetryIntervalInSeconds : DefaultSqlRetryIntervalInSeconds;
            var retryStrategy = new FixedInterval(this.globalSettings.SqlRetryCount > 0 ? this.globalSettings.SqlRetryCount : DefaultSqlRetryCount, TimeSpan.FromSeconds(retryIntervalInSeconds));
            var retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += StoredProcedureExecutionRetryPolicy_Retrying;
            return retryPolicy;
        }
    }
}
