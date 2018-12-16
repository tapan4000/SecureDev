using RestServer.Logging.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestClientCache
{
    public class HttpClientCache : IHttpClientCache
    {
        private IEventLogger logger;

        public HttpClientCache(IEventLogger logger)
        {
            this.logger = logger;
        }

        private static readonly ConcurrentDictionary<string, HttpClient> CachedHttpClientObjects = new ConcurrentDictionary<string, HttpClient>();

        private static readonly ConcurrentDictionary<string, object> ConcurrentLock = new ConcurrentDictionary<string, object>();

        public HttpClient CreateHttpClient(string endpoint)
        {
            var endpointAddress = new Uri(endpoint);
            var host = endpointAddress.Host;

            HttpClient httpClient;
            CachedHttpClientObjects.TryGetValue(host, out httpClient);
            if(null == httpClient)
            {
                lock(ConcurrentLock.GetOrAdd(host, new object()))
                {
                    CachedHttpClientObjects.TryGetValue(host, out httpClient);
                    if(null == httpClient)
                    {
                        this.logger.LogInformation($"Creating HTTP client for host {host}.");
                        httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
                        httpClient.DefaultRequestHeaders.Add("Host", host);
                        if(!CachedHttpClientObjects.TryAdd(host, httpClient))
                        {
                            this.logger.LogError($"Failed to cache HTTP client for host: {host}");
                        }
                    }
                }
            }

            return httpClient;
        }
    }
}
