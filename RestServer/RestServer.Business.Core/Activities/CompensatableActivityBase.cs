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
    public abstract class CompensatableActivityBase<TRequest, TResponse> : Trackable<TRequest, TResponse>, ICompensatableActivity<TRequest, TResponse> where TResponse : RestrictedBusinessResultBase, new()
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

        public async Task<TResponse> ExecuteCompensateAsync()
        {
            var businessResult = new TResponse();
            businessResult.SetSuccessStatus(true);
            
            // TODO: Add code to track the execution time based on the set flags.
            try
            {
                await this.CompensateAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
                businessResult.SetSuccessStatus(false);
            }

            return businessResult;
        }

        protected abstract Task CompensateAsync();
    }
}
