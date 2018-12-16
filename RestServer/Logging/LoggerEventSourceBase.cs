using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Logging
{
    using Microsoft.Diagnostics.Tracing;
    using RestServer.Logging.Interfaces;
    using RestServer.ServerContext;

    public abstract class LoggerEventSourceBase : EventSource, ILoggerEventSource
    {
        [NonEvent]
        public void Verbose(
            string traceId,
            string message,
            int userId,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.VerboseStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                RestServiceContext.ServiceInstanceName,
                message,
                null,
                userId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Info(
            string traceId,
            string message,
            int userId,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.InfoStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                RestServiceContext.ServiceInstanceName,
                message,
                null,
                userId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Warning(
            string traceId,
            string message,
            int userId,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.WarningStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                RestServiceContext.ServiceInstanceName,
                message,
                null,
                userId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Error(
            string traceId,
            string errorMessage,
            int userId,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.ErrorStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                RestServiceContext.ServiceInstanceName,
                errorMessage,
                null,
                userId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Critical(
            string traceId,
            Exception ex,
            int userId,
            string message = null,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.CriticalStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                RestServiceContext.ServiceInstanceName,
                message,
                LogHelper.FlattenException(ex),
                userId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                fileName,
                lineNumber,
                logTime);
        }

        protected abstract void VerboseStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime);

        protected abstract void InfoStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime);

        protected abstract void WarningStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime);

        protected abstract void ErrorStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime);

        protected abstract void CriticalStandardMessage(
            string traceId,
            string serviceName,
            string instanceName,
            string message,
            string errorMessage,
            int userId,
            string logType,
            string dataCenter,
            string memberName,
            string fileName,
            int lineNumber,
            string logTime);
    }
}
