using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Activities
{
    public interface ICompensatableActivity<RequestData, ResponseData> : IActivity<RequestData, ResponseData>
    {
        Task<ResponseData> ExecuteCompensateAsync();
    }
}
