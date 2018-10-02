using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Activities
{
    public interface IActivityFactory<RequestData, ResponseData>
    {
        IActivity<RequestData, ResponseData> CreateActivity<TActivity>() where TActivity : IActivity<RequestData, ResponseData>;

        IActivity<RequestData, ResponseData> CreateGenericActivity();
    }
}
