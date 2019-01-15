using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class ValidateUserRegistrationOtpRequestData : BusinessRequestData
    {
        public int ActivationCode { get; set; }

        public User User { get; set; }
    }
}
