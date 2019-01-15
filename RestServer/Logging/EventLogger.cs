namespace RestServer.Logging
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using RestServer.Logging.Interfaces;
    using System.Text;
    using System.Threading.Tasks;
    using ServerContext;
    using System.Diagnostics;
    using Entities.Interfaces;

    public class EventLogger : IEventLogger
    {
        private readonly IUserContext userContext;

        public EventLogger(IUserContext userContext)
        {
            this.userContext = userContext;
        }

        public void LogInformation(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Info(this.userContext.WorkflowId, message, this.userContext.UserOrServiceIdentifier, memberName, fileName, lineNumber);
        }

        public void LogWarning(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Warning(this.userContext.WorkflowId, message, this.userContext.UserOrServiceIdentifier, memberName, fileName, lineNumber);
        }

        public void LogError(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Error(this.userContext.WorkflowId, message, this.userContext.UserOrServiceIdentifier, memberName, fileName, lineNumber);
        }

        public void LogVerbose(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Verbose(this.userContext.WorkflowId, message, this.userContext.UserOrServiceIdentifier, memberName, fileName, lineNumber);
        }


        public void LogException(string message, Exception ex, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.userContext.WorkflowId, ex, this.userContext.UserOrServiceIdentifier, message, memberName, fileName, lineNumber);
        }

        public void LogException(Exception ex, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.userContext.WorkflowId, ex, this.userContext.UserOrServiceIdentifier, null, memberName, fileName, lineNumber);
        }
    }
}
