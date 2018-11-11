using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Activities
{
    public abstract class CompensatableActivityBase<RequestData, ResponseData> : Trackable<RequestData, ResponseData>, ICompensatableActivity<RequestData, ResponseData> where ResponseData : BusinessResult, new()
    {
        public CompensatableActivityBase(IEventLogger logger) : base(logger)
        {
        }

        public bool IsCompensatable
        {
            get
            {
                return true;
            }
        }

        public abstract Task<bool> CompensateAsync(RequestData requestData);
    }
}
