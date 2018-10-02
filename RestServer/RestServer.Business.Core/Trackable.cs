using RestServer.Business.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core
{
    public abstract class Trackable<RequestData, ResponseData> : ITrackable<RequestData, ResponseData>
    {
        public async Task<ResponseData> TrackAndExecuteAsync(RequestData data)
        {
            // TODO: Add code to track the execution time based on the set flags.
            return await this.ExecuteAsync(data).ConfigureAwait(false);
        }

        public abstract Task<ResponseData> ExecuteAsync(RequestData data);
    }
}
