using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.BaseModels
{
    public class RestrictedBusinessResultBase : BusinessResult
    {
        public override bool IsSuccessful
        {
            get
            {
                return base.IsSuccessful;
            }
            set
            {
                throw new Exception("The property IsSuccessful can be updated only from BusinessResult.");
            }
        }

        public void SetSuccessStatus(bool isSuccessful)
        {
            base.IsSuccessful = isSuccessful;
        }
    }
}
