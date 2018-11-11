using RestServer.Business.Core.BaseModels;
using RestServer.Core.Extensions;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class AddUserRequestData : BusinessRequestData
    {
        public User User { get; set; }

        protected override bool IsValid()
        {
            if(null == this.User || this.User.MobileNumber.IsEmpty() || this.User.Email.IsEmpty() || this.User.EncryptedPassword.IsEmpty() || this.User.FirstName.IsEmpty() || this.User.LastName.IsEmpty())
            {
                return false;
            }

            return true;
        }
    }
}
