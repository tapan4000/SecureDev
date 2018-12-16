using RestServer.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification
{
    public class NotificationQueueSeeder : INotificationQueueSeeder
    {
        public bool QueueEmail()
        {
            // TODO: Add code to seed the email to the email queue. Use a POCO model defined for Email for sending and receiving.
            throw new NotImplementedException();
        }

        public bool QueueSms()
        {
            throw new NotImplementedException();
        }
    }
}
