using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Models
{
    public class UserNotificationInformationRecord
    {
        public int UserId { get; set; }

        public string EmailId { get; set; }

        public string CompleteMobileNumber { get; set; }
    }
}
