using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess
{
    public class StoredProcedureConstants
    {
        public const string SyncAnonymousGroupMemberRequestsStoredProcedure = "usp_SyncAnonymousGroupMemberRequests";

        public const string UserIdParameterName = "UserId";

        public const string UserMobileIsdCodeParameterName = "UserMobileIsdCode";

        public const string UserMobileNumberParameterName = "UserMobileNumber";

        public const string MaxUserCountPerGroupParameterName = "MaxUserCountPerGroup";

        public const string MaxGroupCountPerUserParameterName = "MaxGroupCountPerUser";
    }
}
