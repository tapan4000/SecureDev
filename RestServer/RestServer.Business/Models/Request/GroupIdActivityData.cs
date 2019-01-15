using RestServer.Business.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class GroupIdActivityData : BusinessRequestData
    {
        public int GroupId { get; set; }
    }
}
