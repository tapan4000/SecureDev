using RestServer.Business.Core.BaseModels;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class SendNotificationActivityData : BusinessRequestData
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public string RecipientIdentifier { get; set; }

        public NotificationModeEnum NotificationMode { get; set; }

        public bool CanNotificationFailureCauseFlowFailure { get; set; }
    }
}
