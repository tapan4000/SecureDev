using RestServer.Business.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Response
{
    public class AddUserRequestBusinessResult : RestrictedBusinessResultBase
    {
        public string UserAuthToken { get; set; }

        public DateTime AuthTokenExpirationDateTime { get; set; }

        public string FirstName { get; set; }

        public string UserLoginRefreshToken { get; set; }

        public long RefreshTokenCreationDateTime { get; set; }
    }
}
