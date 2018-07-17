using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FabricShared.Logging.Logging
{
    using System.Diagnostics.Tracing;
    using System.Fabric;

    //[EventSource(Name="Fabric-Log")]
    //public class ServiceLoggerEventSource : EventSource, IServiceLoggerEventSource
    //{
    //    public void Verbose(
    //        Guid traceId,
    //        string message,
    //        string memberName = null,
    //        string fileName = null,
    //        int lineNumber = 0,
    //        string logTime = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Info(
    //        Guid traceId,
    //        string message,
    //        string memberName = null,
    //        string fileName = null,
    //        int lineNumber = 0,
    //        string logTime = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Error(
    //        Guid traceId,
    //        string errorMessage,
    //        string memberName = null,
    //        string fileName = null,
    //        int lineNumber = 0,
    //        string logTime = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Exception(
    //        Guid traceId,
    //        Exception ex,
    //        string memberName = null,
    //        string fileName = null,
    //        int lineNumber = 0,
    //        string logTime = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void ServiceMessage(ServiceContext serviceContext, string message, params object[] args)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
