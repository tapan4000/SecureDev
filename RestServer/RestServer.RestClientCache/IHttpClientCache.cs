using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestClientCache
{
    public interface IHttpClientCache
    {
        HttpClient CreateHttpClient(string endpoint);
    }
}
