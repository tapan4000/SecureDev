namespace RestServer.Logging.Interfaces
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public interface IEventLogger
    {
        void LogInformation(
            string message,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");

        void LogWarning(
            string message,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");

        void LogError(
            string message,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");

        void LogVerbose(
            string message,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");

        void LogException(
            string message,
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");

        void LogException(
            Exception ex,
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "");
    }
}
