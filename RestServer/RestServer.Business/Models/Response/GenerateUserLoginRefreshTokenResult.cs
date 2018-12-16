using RestServer.Business.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Response
{
    public class GenerateUserLoginRefreshTokenResult : RestrictedBusinessResultBase
    {
        public string RefreshToken { get; set; }

        public long RefreshTokenCreationDateTime { get; set; }
    }
}
