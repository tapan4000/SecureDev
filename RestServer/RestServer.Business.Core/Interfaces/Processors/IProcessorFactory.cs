using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Processors
{
    public interface IProcessorFactory
    {
        ITrackable<RequestData, ResponseData> CreateProcessor<TProcessor, RequestData, ResponseData>() where TProcessor : ITrackable<RequestData, ResponseData>;

        ITrackable<RequestData, ResponseData> CreateGenericProcessor<TActivity, RequestData, ResponseData>() where TActivity : ITrackable<RequestData, ResponseData>;
    }
}
