using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification.Interfaces
{
    public interface INotificationQueueSeeder
    {
        bool QueueEmail();

        bool QueueSms();
    }
}
