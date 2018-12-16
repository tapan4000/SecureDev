using RestServer.Configuration;
using RestServer.Configuration.Interfaces;
using RestServer.Core.Extensions;
using RestServer.Notification.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private IConfigurationHandler configurationHandler;

        public EmailProvider(IConfigurationHandler configurationHandler)
        {
            this.configurationHandler = configurationHandler;
        }

        public async Task<bool> SendEmailAsync(string subject, string body, string targetEmailAddress)
        {
            var apiKey = await this.configurationHandler.GetConfiguration(ConfigurationConstants.SendGridApiKey);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("accounts@restserver.com", "Rest Server");
            var to = new EmailAddress(targetEmailAddress);
            var plainTextContent = body;
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var htmlContent = body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            var errorMessage = await response.Body.ReadAsStringAsync();
            return response.StatusCode == System.Net.HttpStatusCode.Accepted && errorMessage.IsEmpty();
        }
    }
}
