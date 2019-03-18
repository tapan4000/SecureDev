using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess
{
    public class StoredProcedureConstants
    {
        // Procedure Name constants
        public const string SyncAnonymousGroupMemberRequestsStoredProcedure = "usp_SyncAnonymousGroupMemberRequests";

        public const string FetchNotificationDetailsForAdminsByGroupStoredProcedure = "usp_FetchNotificationDetailsForAdminsByGroup";

        // Stored procedure parameter or result set constants.
        public const string UserId = "UserId";

        public const string UserMobileIsdCode = "UserMobileIsdCode";

        public const string UserMobileNumber = "UserMobileNumber";

        public const string MaxUserCountPerGroup = "MaxUserCountPerGroup";

        public const string MaxGroupCountPerUser = "MaxGroupCountPerUser";

        public const string GroupId = "GroupId";

        public const string CompleteMobileNumber = "CompleteMobileNumber";

        public const string EmailId = "EmailId";
    }
}
