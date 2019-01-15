using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.StoredProcedureAccessStrategies;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Cache.Interfaces;
using RestServer.Configuration.Interfaces;
using RestServer.Core.Extensions;

namespace RestServer.DataAccess.StoredProcedureAccessStrategies
{
    public class GroupStoredProcedureAccessSqlStrategy : SqlStoredProcedureAccessStrategyBase, IGroupStoredProcedureAccessStrategy
    {
        public GroupStoredProcedureAccessSqlStrategy(IConfigurationHandler configurationHandler, IEventLogger logger, IConsolidatedCacheInvalidator cacheInvalidator) : base(configurationHandler, logger, cacheInvalidator)
        {
        }

        public async Task SyncAnonymousGroupMemberRequests(int userId, string userIsdCode, string mobileNumber, int maxUserCountPerGroup, int maxGroupCountPerUser)
        {
            var parameters = new Dictionary<string, string>
            {
                { StoredProcedureConstants.UserIdParameterName, userId.ToString() },
                { StoredProcedureConstants.UserMobileIsdCodeParameterName, userIsdCode },
                { StoredProcedureConstants.UserMobileNumberParameterName, mobileNumber },
                { StoredProcedureConstants.MaxUserCountPerGroupParameterName, maxUserCountPerGroup.ToString() },
                { StoredProcedureConstants.MaxGroupCountPerUserParameterName, maxGroupCountPerUser.ToString() }
            };

            var logResponse = await this.ExecuteProcedure<string>(StoredProcedureConstants.SyncAnonymousGroupMemberRequestsStoredProcedure, parameters);
            if (!logResponse.IsEmpty())
            {
                this.Logger.LogInformation(logResponse);
            }
        }
    }
}
