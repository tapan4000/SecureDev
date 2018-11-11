using RestServer.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.BaseModels
{
    public class BusinessResult
    {
        public bool IsSuccessful { get; set; }

        public List<BusinessError> BusinessErrors { get; private set; }

        public void AddBusinessError(BusinessErrorCode errorCode)
        {
            this.BusinessErrors.Add(new BusinessError
            {
                ErrorCode = errorCode
            });
        }
    }
}
