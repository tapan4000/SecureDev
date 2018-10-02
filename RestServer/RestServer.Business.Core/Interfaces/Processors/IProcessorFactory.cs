using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Interfaces.Processors
{
    public interface IProcessorFactory<RequestData, ResponseData>
    {
        IProcessor<RequestData, ResponseData> CreateProcessor<TProcessor>() where TProcessor : IProcessor<RequestData, ResponseData>;

        IProcessor<RequestData, ResponseData> CreateGenericProcessor();
    }
}
