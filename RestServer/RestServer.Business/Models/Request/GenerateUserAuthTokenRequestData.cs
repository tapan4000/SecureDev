using RestServer.Business.Core.BaseModels;
using RestServer.RestSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class GenerateUserAuthTokenRequestData : BusinessRequestData
    {
        public int UserId { get; set; }

        public string ApplicationUniqueId { get; set; }

        public string UserUniqueId { get; set; }
    }
}
