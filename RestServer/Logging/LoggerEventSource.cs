namespace RestServer.Logging
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Tracing;

    using RestServer.Logging.Interfaces;

    [EventSource(Name="Event-Log")]
    public class LoggerEventSource : EventSource, ILoggerEventSource
    {
        private static readonly Lazy<LoggerEventSource> Instance =
            new Lazy<LoggerEventSource>(() => new LoggerEventSource());

        public static LoggerEventSource Current => Instance.Value;

        public void Verbose(
            string traceId,
            string message,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            Trace.Write(message);
        }

        public void Info(
            string traceId,
            string message,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            Trace.Write(message);
        }

        public void Error(
            string traceId,
            string errorMessage,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            Trace.Write(errorMessage);
        }

        public void Exception(
            string traceId,
            Exception ex,
            string memberName = null,
            string fileName = null,
            int lineNumber = 0,
            string logTime = null)
        {
            Trace.Write(ex.Message);
        }
    }
}
