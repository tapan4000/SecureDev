using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces
{
    public interface ITrackable<RequestData, ResponseData>
    {
        Task<ResponseData> TrackAndExecuteAsync(RequestData data);
    }
}
