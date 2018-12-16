using RestServer.Business.Core.BaseModels;
using RestServer.RestSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Response
{
    public class GenerateUserAuthTokenResult : RestrictedBusinessResultBase
    {
        public string EncodedSignedToken { get; set; }

        public DateTime AuthTokenExpirationDateTime { get; set; }
    }
}
