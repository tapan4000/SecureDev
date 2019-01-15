using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.DocumentDb
{
    public class DocumentException : Exception
    {
        public string StatusCode;

        public DocumentException(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode.ToString();
        }
    }
}
