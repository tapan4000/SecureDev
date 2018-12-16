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
        private List<BusinessError> businessErrors;
        public BusinessResult()
        {
            this.businessErrors = new List<BusinessError>();
        }

        public virtual bool IsSuccessful { get; set; }

        public List<BusinessError> BusinessErrors
        {
            get
            {
                return this.businessErrors;
            }
        }

        public void AddBusinessError(BusinessErrorCode errorCode)
        {
            if(!this.businessErrors.Any(businessError => businessError.ErrorCode == errorCode))
            {
                this.businessErrors.Add(new BusinessError
                {
                    ErrorCode = errorCode
                });
            }
        }

        public void AppendBusinessErrors(List<BusinessError> businessErrors)
        {
            foreach(var businessError in businessErrors)
            {
                this.AddBusinessError(businessError.ErrorCode);
            }
        }
    }
}
