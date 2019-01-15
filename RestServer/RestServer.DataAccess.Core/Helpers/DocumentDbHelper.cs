using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Helpers
{
    public static class DocumentDbHelper
    {
        private const string UserLocationIdPrefix = "Loc_";

        private const string UserBlockListIdPrefix = "Ubl_";

        public static string GetUserDocumentPartitionKey(int userId)
        {
            return userId.ToString();
        }

        public static string GetUserBlockListDocumentId(int userId)
        {
            return UserBlockListIdPrefix + userId;
        }

        public static string GetUserLocationDocumentId(int userId)
        {
            return UserLocationIdPrefix + userId;
        }
    }
}
