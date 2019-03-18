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
        public IList<NotificationRecipient> Recipients { get; set; }

        public NotificationMessageTypeEnum NotificationMessageType { get; set; }

        public bool CanNotificationFailureCauseFlowFailure { get; set; }

        public IList<KeyValuePair<string, string>> SubjectMergeFields { get; set; }

        public IList<KeyValuePair<string, string>> BodyMergeFields { get; set; }
    }
}
