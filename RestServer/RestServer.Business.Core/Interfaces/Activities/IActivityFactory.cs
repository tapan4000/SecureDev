using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Activities
{
    public interface IActivityFactory
    {
        IActivity<TActivityRequest, TActivityResponse> CreateActivity<TActivity, TActivityRequest, TActivityResponse>() where TActivity : IActivity<TActivityRequest, TActivityResponse>;

        IActivity<TActivityRequest, TActivityResponse> CreateGenericActivity<TActivityRequest, TActivityResponse>();
    }
}
