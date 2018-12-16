using RestServer.Business.Core.Interfaces;
using RestServer.Business.Core.Interfaces.Processors;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Processors
{
    public class ProcessorFactory : IProcessorFactory
    {
        private readonly IDependencyContainer dependencyContainer;

        public ProcessorFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public ITrackable<RequestData, ResponseData> CreateGenericProcessor<TActivity, RequestData, ResponseData>() where TActivity : ITrackable<RequestData, ResponseData>
        {
            var activityName = typeof(TActivity).Name;
            var activity = this.dependencyContainer.Resolve<TActivity>(activityName);

            if(null == activity)
            {
                throw new ArgumentException($"Failed to create named activity: {activityName}.");
            }

            return activity;
        }

        public ITrackable<RequestData, ResponseData> CreateProcessor<TProcessor, RequestData, ResponseData>() where TProcessor : ITrackable<RequestData, ResponseData>
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
