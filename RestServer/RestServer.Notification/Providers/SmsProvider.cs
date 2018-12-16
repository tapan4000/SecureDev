using Newtonsoft.Json;
using RestServer.Configuration;
using RestServer.Configuration.Interfaces;
using RestServer.Notification.Interfaces;
using RestServer.Notification.Models;
using RestServer.RestClientCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Notification.Providers
{
    public class SmsProvider : ISmsProvider
    {
        private IHttpClientCache httpClientCache;

        private IConfigurationHandler configurationHandler;

        public SmsProvider(IHttpClientCache httpClientCache, IConfigurationHandler configurationHandler)
        {
            this.httpClientCache = httpClientCache;
            this.configurationHandler = configurationHandler;
        }

        public async Task<bool> SendSms(string message, string mobileNumber)
        {
            var smsHttpClient = this.httpClientCache.CreateHttpClient("https://api.textlocal.in");
            var postParams = new Dictionary<string, string>();

            var apiKey = await this.configurationHandler.GetConfiguration(ConfigurationConstants.TextLocalSmsApiKey).ConfigureAwait(false);
            postParams.Add("apikey", apiKey);
            postParams.Add("numbers", mobileNumber);
            postParams.Add("sender", "RSTSVR");
            //postParams.Add("apikey", "AOSEYgmX0Z0-0nrCeN80lSDcHlUtgxQZPJ0qKUbmHk");
            //postParams.Add("numbers", "8886333025");
            postParams.Add("message", message);

            var postContent = new FormUrlEncodedContent(postParams);

            HttpResponseMessage response = await smsHttpClient.PostAsync("https://api.textlocal.in/send/", postContent).ConfigureAwait(false);
            var jsonContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var smsResponse = JsonConvert.DeserializeObject<TextLocalSmsResponseModel>(jsonContent);
            return smsResponse.Status.Equals("success", StringComparison.OrdinalIgnoreCase);
        }
    }
}
