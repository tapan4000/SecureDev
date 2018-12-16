using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestServer.FrontEndService
{
    public class RestServerHttpActionResult : IHttpActionResult
    {
        private HttpStatusCode statusCode;

        public RestServerHttpActionResult(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(this.statusCode));
        }
    }
}
