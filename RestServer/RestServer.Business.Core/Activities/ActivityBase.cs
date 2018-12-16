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
    public abstract class ActivityBase<RequestData, ResponseData> : Trackable<RequestData, ResponseData>, IActivity<RequestData, ResponseData> where ResponseData : RestrictedBusinessResultBase, new()
    {
        public ActivityBase(IEventLogger logger) : base(logger)
        {
        }

        public bool IsCompensatable
        {
            get
            {
                return false;
            }
        }
    }
}
