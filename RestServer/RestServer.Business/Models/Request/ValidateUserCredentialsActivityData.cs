using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class ValidateUserCredentialsActivityData : BusinessRequestData
    {
        public string IsdCode { get; set; }

        public string MobileNumber { get; set; }

        public string PasswordHash { get; set; }
    }
}
