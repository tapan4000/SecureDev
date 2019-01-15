using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Interfaces;
using RestServer.Business.Models;
using RestServer.Logging.Interfaces;
using RestServer.ServerContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core
{
    public abstract class Trackable<TRequest, TResponse> : ITrackable<TRequest, TResponse> where TResponse : RestrictedBusinessResultBase, new()
    {
        protected BusinessResult Result;

        // If this field is set then any failure of the trackable can be ignored.
        protected bool CanIgnoreTrackableFailure = false;

        public Trackable(IEventLogger logger)
        {
            this.logger = logger;
        }

        protected IEventLogger logger;
        public async Task<TResponse> TrackAndExecuteAsync(TRequest requestData)
        {
            this.Result = new BusinessResult { IsSuccessful = true };
            TResponse businessFlowResult;
            // TODO: Add code to track the execution time based on the set flags.
            if (RestServiceContext.IsExecutionTimeLoggingEnabled)
            {
                var stopWatch = Stopwatch.StartNew();
                businessFlowResult = await ValidateAndExecuteAsync(requestData).ConfigureAwait(false);
                stopWatch.Stop();
                
                // TODO: Make sure the activity name or processor name is logged as part of this informational message.
                this.logger.LogInformation($"Time taken: {stopWatch.ElapsedMilliseconds}.");
            }
            else
            {
                businessFlowResult = await ValidateAndExecuteAsync(requestData).ConfigureAwait(false);
            }

            // For scenarios where the execution of the method failed, the business flow result may be null. For such scenario, assign a new instance to business flow result.
            if (null == businessFlowResult)
            {
                businessFlowResult = new TResponse();
            }

            businessFlowResult.SetSuccessStatus(this.CanIgnoreTrackableFailure ? true : this.Result.IsSuccessful);
            businessFlowResult.AppendBusinessErrors(this.Result.BusinessErrors);
            return businessFlowResult;
        }

        private async Task<TResponse> ValidateAndExecuteAsync(TRequest requestData)
        {
            try
            {
                if (!this.ValidateRequestData(requestData))
                {
                    this.Result.IsSuccessful = false;
                    return default(TResponse);
                }

                return await this.ExecuteAsync(requestData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
                this.Result.IsSuccessful = false;
            }

            return default(TResponse);
        }

        protected abstract Task<TResponse> ExecuteAsync(TRequest requestData);

        protected abstract bool ValidateRequestData(TRequest requestData);
    }
}
