using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Enums
{
    public enum NotificationMessageTypeEnum
    {
        None = 0,
        UserWelcomeMessage = 1,
        GroupJoinRequestCreated = 2,
        UserRegistrationOtp = 3,
        EmergencySessionNotificationToAdmins = 4,
    }
}
