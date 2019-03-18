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
        private readonly IActivityFactory activityFactory;

        public ActivityBase(IActivityFactory activityFactory, IEventLogger logger) : base(logger)
        {
            this.activityFactory = activityFactory;
        }

        public bool IsCompensatable
        {
            get
            {
                return false;
            }
        }

        protected async Task<TActivityResponse> CreateAndExecuteActivity<TActivity, TActivityRequest, TActivityResponse>(TActivityRequest requestData) where TActivity : IActivity<TActivityRequest, TActivityResponse> where TActivityResponse : RestrictedBusinessResultBase, new()
        {
            // An activity can be called from a processable activity alone as the compensation of inner activities cannot be controlled using compensatable activity.
            var activity = this.activityFactory.CreateActivity<TActivity, TActivityRequest, TActivityResponse>();
            var activityResult = await activity.TrackAndExecuteAsync(requestData);

            // For a successful activity no need to modify the business processor result IsSuccessful flag as it would be true originally. Only in case of a failure scenario modify the
            // flag and append the errors.
            if (!activityResult.IsSuccessful)
            {
                this.Result.IsSuccessful = false;
                this.Result.AppendBusinessErrors(activityResult.BusinessErrors);
            }

            return activityResult;
        }
    }
}
