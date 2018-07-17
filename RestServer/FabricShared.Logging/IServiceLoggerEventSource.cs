using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FabricShared.Logging
{
    using System.Fabric;
    using System.Runtime.CompilerServices;

    //public interface IServiceLoggerEventSource
    //{
    //    void Verbose(
    //        Guid traceId,
    //        string message,
    //        [CallerMemberName] string memberName = null,
    //        [CallerFilePath] string fileName = null,
    //        [CallerLineNumber] int lineNumber = 0,
    //        string logTime = null);

    //    void Info(
    //        Guid traceId,
    //        string message,
    //        [CallerMemberName] string memberName = null,
    //        [CallerFilePath] string fileName = null,
    //        [CallerLineNumber] int lineNumber = 0,
    //        string logTime = null);

    //    void Error(
    //        Guid traceId,
    //        string errorMessage,
    //        [CallerMemberName] string memberName = null,
    //        [CallerFilePath] string fileName = null,
    //        [CallerLineNumber] int lineNumber = 0,
    //        string logTime = null);

    //    void Exception(
    //        Guid traceId,
    //        Exception ex,
    //        [CallerMemberName] string memberName = null,
    //        [CallerFilePath] string fileName = null,
    //        [CallerLineNumber] int lineNumber = 0,
    //        string logTime = null);

    //    void ServiceMessage(ServiceContext serviceContext, string message, params object[] args);
    //}
}
