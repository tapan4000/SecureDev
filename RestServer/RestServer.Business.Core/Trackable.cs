using RestServer.Business.Core.BaseModels;
using RestServer.Business.Core.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core
{
    public abstract class Trackable<RequestData, ResponseData> : ITrackable<RequestData, ResponseData> where ResponseData : BusinessResult, new()
    {
        protected ResponseData Result;

        public Trackable(IEventLogger logger)
        {
            this.logger = logger;
        }

        protected IEventLogger logger;
        public async Task<ResponseData> TrackAndExecuteAsync(RequestData requestData)
        {
            this.Result = new ResponseData { IsSuccessful = true };

            // TODO: Add code to track the execution time based on the set flags.
            try
            {
                if (!this.ValidateRequestData(requestData))
                {
                    this.Result.IsSuccessful = false;
                    return this.Result;
                }

                await this.ExecuteAsync(requestData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
                this.Result.IsSuccessful = false;
            }

            return this.Result;
        }

        public abstract Task ExecuteAsync(RequestData requestData);

        public abstract bool ValidateRequestData(RequestData requestData);
    }
}
