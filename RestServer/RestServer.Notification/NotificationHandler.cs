using RestServer.Notification.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification
{
    public class NotificationHandler : INotificationHandler
    {
        private IEmailProvider emailProvider;

        private ISmsProvider smsProvider;

        public NotificationHandler(IEmailProvider emailProvider, ISmsProvider smsProvider)
        {
            this.emailProvider = emailProvider;
            this.smsProvider = smsProvider;
        }

        public async Task<bool> SendEmail(string subject, string body, string targetEmailAddress)
        {
            return await this.emailProvider.SendEmailAsync(subject, body, targetEmailAddress);
        }

        public async Task<bool> SendSms(string message, string completeMobileNumber)
        {
            return await this.smsProvider.SendSms(message, completeMobileNumber).ConfigureAwait(false);
        }
    }
}
