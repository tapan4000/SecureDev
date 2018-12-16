using RestServer.Business.Core.BaseModels;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Models.Response
{
    public class PopulatedGroupBusinessResult : RestrictedBusinessResultBase
    {
        public Group Group { get; set; }
    }
}
