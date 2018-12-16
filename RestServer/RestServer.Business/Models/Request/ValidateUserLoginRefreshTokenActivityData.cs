using RestServer.Business.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class ValidateUserLoginRefreshTokenActivityData : BusinessRequestData
    {
        public int UserId { get; set; }

        public string RefreshToken { get; set; }

        public long TokenCreationDateTime { get; set; }
    }
}
