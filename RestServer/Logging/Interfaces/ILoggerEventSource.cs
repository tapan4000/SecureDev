namespace RestServer.Logging.Interfaces
{
    using System;
    using System.Runtime.CompilerServices;

    public interface ILoggerEventSource
    {
        void Verbose(
            string traceId,
            string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Info(
            string traceId,
            string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Error(
            string traceId,
            string errorMessage,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Exception(
            string traceId,
            Exception ex,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);
    }
}
