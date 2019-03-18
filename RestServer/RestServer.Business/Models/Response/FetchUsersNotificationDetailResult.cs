using RestServer.Business.Core.BaseModels;
using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Response
{
    public class FetchUsersNotificationDetailResult : RestrictedBusinessResultBase
    {
        public IList<UserNotificationInformationRecord> NotificationDetailForAdmins { get; set; }
    }
}
