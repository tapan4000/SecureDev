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
using RestServer.DataAccess.Core.Models;
using System.Data;

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
                { StoredProcedureConstants.UserId, userId.ToString() },
                { StoredProcedureConstants.UserMobileIsdCode, userIsdCode },
                { StoredProcedureConstants.UserMobileNumber, mobileNumber },
                { StoredProcedureConstants.MaxUserCountPerGroup, maxUserCountPerGroup.ToString() },
                { StoredProcedureConstants.MaxGroupCountPerUser, maxGroupCountPerUser.ToString() }
            };

            var logResponse = await this.ExecuteProcedure<string>(StoredProcedureConstants.SyncAnonymousGroupMemberRequestsStoredProcedure, parameters);
            if (!logResponse.IsEmpty())
            {
                this.Logger.LogInformation(logResponse);
            }
        }

        public async Task<IList<UserNotificationInformationRecord>> FetchNotificationDetailsForAdminsByGroup(int groupId)
        {
            var parameters = new Dictionary<string, string>
            {
                { StoredProcedureConstants.GroupId, groupId.ToString() }
            };

            var adminNotificationDetailsDataTable = await this.ExecuteProcedureReturningTable(StoredProcedureConstants.FetchNotificationDetailsForAdminsByGroupStoredProcedure, parameters);
            if(adminNotificationDetailsDataTable.Rows.Count == 0)
            {
                return null;
            }

            var userNotificationRecords = new List<UserNotificationInformationRecord>();
            foreach(DataRow adminNotificationDetailRow in adminNotificationDetailsDataTable.Rows)
            {
                userNotificationRecords.Add(new UserNotificationInformationRecord
                {
                    CompleteMobileNumber = adminNotificationDetailRow[StoredProcedureConstants.CompleteMobileNumber] != DBNull.Value
                        ? Convert.ToString(adminNotificationDetailRow[StoredProcedureConstants.CompleteMobileNumber]) : null,
                    EmailId = adminNotificationDetailRow[StoredProcedureConstants.EmailId] != DBNull.Value
                        ? Convert.ToString(adminNotificationDetailRow[StoredProcedureConstants.EmailId]) : null,
                    UserId = adminNotificationDetailRow[StoredProcedureConstants.UserId] != DBNull.Value
                        ? Convert.ToInt32(adminNotificationDetailRow[StoredProcedureConstants.UserId]) : 0,
                });
            }

            return userNotificationRecords;
        }
    }
}
