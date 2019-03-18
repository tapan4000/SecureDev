using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Interfaces.StoredProcedureAccessStrategies
{
    public interface IGroupStoredProcedureAccessStrategy
    {
        Task SyncAnonymousGroupMemberRequests(int userId, string userIsdCode, string mobileNumber, int maxUserCountPerGroup, int maxGroupCountPerUser);

        Task<IList<UserNotificationInformationRecord>> FetchNotificationDetailsForAdminsByGroup(int groupId);
    }
}
