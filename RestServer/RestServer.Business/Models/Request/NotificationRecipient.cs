using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class NotificationRecipient
    {
        public string CompleteMobileNumber { get; set; }

        public string EmailId { get; set; }

        public NotificationModeEnum? NotificationPreference { get; set; }
    }
}
