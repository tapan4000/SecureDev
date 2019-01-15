using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification
{
    public class NotificationTemplateConstants
    {
        public class SmsConstants
        {
            public const string UserRegistrationOtp = "{0} is the OTP for the registration request.";

            public const string GroupJoinRequest = "You have a request from {0} to join the group {1}. Please visit the pending request section to approve/deny the request.";
        }

        public class EmailConstants
        {
            public const string UserRegistrationOtpEmailSubject = "OTP for registration request.";

            public const string UserRegistrationOtpEmailBody = "{0} is the OTP for the registration request.";

            public const string GroupJoinRequestEmailSubject = "You have a request to join a group.";

            public const string GroupJoinRequestEmailBody = "You have a request from {0} to join the group {1}. Please visit the pending request section to approve/deny the request.";
        }
    }
}
