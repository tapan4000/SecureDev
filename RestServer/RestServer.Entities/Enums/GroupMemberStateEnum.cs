using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Enums
{
    public enum GroupMemberStateEnum
    {
        None = 0,
        PendingAcceptance = 1,
        PendingRequestForUpgradeToAdmin = 2,
        Accepted = 3,
        Rejected = 4,
        RequestDeletedByAdmin = 5
    }
}
