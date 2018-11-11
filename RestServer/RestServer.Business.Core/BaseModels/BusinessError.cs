using RestServer.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.BaseModels
{
    public class BusinessError
    {
        public BusinessErrorCode ErrorCode { get; set; }
    }
}
