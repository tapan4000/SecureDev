using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Business.Core.Interfaces.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Processors
{
    public abstract class ProcessorBase<RequestData, ResponseData> : Trackable<RequestData, ResponseData>
    {
        private readonly IActivityFactory<RequestData, ResponseData> activityFactory;

        private readonly Stack<IActivity<RequestData, ResponseData>> compensatableActivities;

        protected ProcessorBase(IActivityFactory<RequestData, ResponseData> activityFactory)
        {
            this.compensatableActivities = new Stack<IActivity<RequestData, ResponseData>>();
            this.activityFactory = activityFactory;
        }

        protected Task<ResponseData> ExecuteActivityAsync<TActivity>(RequestData activityData) where TActivity : IActivity<RequestData, ResponseData>
        {
            var activity = this.activityFactory.CreateActivity<TActivity>();
            if (activity.IsCompensatable)
            {
                this.compensatableActivities.Push((ICompensatableActivity<RequestData, ResponseData>)activity);
            }

            return activity.TrackAndExecuteAsync(activityData);
        }
    }
}
