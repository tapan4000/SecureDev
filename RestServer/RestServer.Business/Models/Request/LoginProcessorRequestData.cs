using RestServer.Business.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class LoginProcessorRequestData : BusinessRequestData
    {
        public string IsdCode { get; set; }

        public string MobileNumber { get; set; }

        public string PasswordHash { get; set; }

        public string ApplicationUniqueId { get; set; }

        public bool IsTokenBasedLogin { get; set; }

        public string RefreshToken { get; set; }

        public long TokenCreationDateTime { get; set; }

        public int UserId { get; set; }
    }
}
