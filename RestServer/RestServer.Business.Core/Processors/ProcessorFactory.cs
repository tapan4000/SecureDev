using RestServer.Business.Core.Interfaces.Processors;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Processors
{
    public class ProcessorFactory<RequestData, ResponseData> : IProcessorFactory<RequestData, ResponseData>
    {
        private readonly IDependencyContainer dependencyContainer;

        public ProcessorFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IProcessor<RequestData, ResponseData> CreateGenericProcessor()
        {
            throw new NotImplementedException();
        }

        public IProcessor<RequestData, ResponseData> CreateProcessor<TProcessor>() where TProcessor:IProcessor<RequestData, ResponseData>
        {
            var processorName = typeof(TProcessor).Name;
            var processor = this.dependencyContainer.Resolve<TProcessor>(processorName);

            if(null == processor)
            {
                throw new ArgumentException($"Failed to create named processor: {processorName}.");
            }

            return processor;
        }
    }
}
