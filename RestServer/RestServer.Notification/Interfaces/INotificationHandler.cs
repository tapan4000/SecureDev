using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification.Interfaces
{
    public interface INotificationHandler
    {
        Task<bool> SendEmail(string subject, string body, string targetEmailAddress);

        Task<bool> SendSms(string message, string completeMobileNumber);
    }
}
