namespace RestServer.Logging.Interfaces
{
    using System;
    using System.Runtime.CompilerServices;

    public interface ILoggerEventSource
    {
        void Verbose(
            string traceId,
            string message,
            int userId,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Info(
            string traceId,
            string message,
            int userId,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Warning(
            string traceId,
            string message,
            int userId,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Error(
            string traceId,
            string errorMessage,
            int userId,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);

        void Critical(
            string traceId,
            Exception ex,
            int userId,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            string logTime = null);
    }
}
