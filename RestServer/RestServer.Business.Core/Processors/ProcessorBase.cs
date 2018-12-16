using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Interfaces.Activities;
using RestServer.Business.Core.Interfaces.Processors;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Processors
{
    public abstract class ProcessorBase<TRequest, TResponse> : Trackable<TRequest, TResponse> where TResponse : RestrictedBusinessResultBase, new()
    {
        private readonly IActivityFactory activityFactory;

        private readonly Stack<ICompensatableActivity<TRequest, TResponse>> compensatableActivities;

        protected ProcessorBase(IActivityFactory activityFactory, IEventLogger logger) : base(logger)
        {
            this.compensatableActivities = new Stack<ICompensatableActivity<TRequest, TResponse>>();
            this.activityFactory = activityFactory;
        }

        protected async Task<BusinessResult> CompensateProcessor()
        {
            var compensationResult = new RestrictedBusinessResultBase();
            compensationResult.SetSuccessStatus(true);
            foreach (var compensatableActivity in this.compensatableActivities)
            {
                var activityResult = await compensatableActivity.ExecuteCompensateAsync();
                compensationResult.SetSuccessStatus(compensationResult.IsSuccessful && activityResult.IsSuccessful);
            }

            return compensationResult;
        }

        protected async Task<TActivityResponse> CreateAndExecuteActivity<TActivity, TActivityRequest, TActivityResponse>(TActivityRequest requestData) where TActivity : IActivity<TActivityRequest, TActivityResponse> where TActivityResponse : RestrictedBusinessResultBase, new()
        {
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
