using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class ContextUserIdActivityData : BusinessRequestData
    {
        public int UserId { get; set; }

        public User UserInContext { get; set; } 
    }
}
