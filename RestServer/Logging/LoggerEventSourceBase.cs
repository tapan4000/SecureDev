using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Logging
{
    using System.Diagnostics.Tracing;

    using RestServer.Logging.Interfaces;
    using RestServer.ServerContext;

    public abstract class LoggerEventSourceBase : EventSource, ILoggerEventSource
    {
        [NonEvent]
        public void Verbose(
            string traceId,
            string message,
            string userUniqueId,
            string memberName = null,
            string filePath = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.VerboseStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                message,
                null,
                userUniqueId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                filePath,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Info(
            string traceId,
            string message,
            string userUniqueId,
            string memberName = null,
            string filePath = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.InfoStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                message,
                null,
                userUniqueId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                filePath,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Warning(
            string traceId,
            string message,
            string userUniqueId,
            string memberName = null,
            string filePath = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.WarningStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                message,
                null,
                userUniqueId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                filePath,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Error(
            string traceId,
            string errorMessage,
            string userUniqueId,
            string memberName = null,
            string filePath = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.ErrorStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                errorMessage,
                null,
                userUniqueId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                filePath,
                lineNumber,
                logTime);
        }

        [NonEvent]
        public void Critical(
            string traceId,
            Exception ex,
            string userUniqueId,
            string message = null,
            string memberName = null,
            string filePath = null,
            int lineNumber = 0,
            string logTime = null)
        {
            this.CriticalStandardMessage(
                traceId,
                RestServiceContext.ServiceName,
                message,
                LogHelper.FlattenException(ex),
                userUniqueId,
                null,
                RestServiceContext.DataCenter,
                memberName,
                filePath,
                lineNumber,
                logTime);
        }

        protected abstract void VerboseStandardMessage(
            string traceId,
            string serviceName,
            string message,
            string errorMessage,
            string userUniqueId,
            string logType,
            string dataCenter,
            string memberName,
            string filePath,
            int lineNumber,
            string logTime);

        protected abstract void InfoStandardMessage(
            string traceId,
            string serviceName,
            string message,
            string errorMessage,
            string userUniqueId,
            string logType,
            string dataCenter,
            string memberName,
            string filePath,
            int lineNumber,
            string logTime);

        protected abstract void WarningStandardMessage(
            string traceId,
            string serviceName,
            string message,
            string errorMessage,
            string userUniqueId,
            string logType,
            string dataCenter,
            string memberName,
            string filePath,
            int lineNumber,
            string logTime);

        protected abstract void ErrorStandardMessage(
            string traceId,
            string serviceName,
            string message,
            string errorMessage,
            string userUniqueId,
            string logType,
            string dataCenter,
            string memberName,
            string filePath,
            int lineNumber,
            string logTime);

        protected abstract void CriticalStandardMessage(
            string traceId,
            string serviceName,
            string message,
            string errorMessage,
            string userUniqueId,
            string logType,
            string dataCenter,
            string memberName,
            string filePath,
            int lineNumber,
            string logTime);
    }
}
