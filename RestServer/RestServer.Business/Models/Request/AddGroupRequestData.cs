using RestServer.Business.Core.BaseModels;
using RestServer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Request
{
    public class AddGroupRequestData : BusinessRequestData
    {
        public GroupCategoryEnum GroupCategoryId { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPrimary { get; set; }

        public int UserId { get; set; }
    }
}
