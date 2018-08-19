namespace RestServer.Logging
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using RestServer.Logging.Interfaces;
    using System.Text;

    public class EventLogger : IEventLogger
    {
        private readonly IWorkflowContext workflowContext;

        public EventLogger(IWorkflowContext workflowContext)
        {
            this.workflowContext = workflowContext;
        }

        public void LogInformation(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Info(this.workflowContext.WorkflowId, message, this.workflowContext.UserUniqueId, memberName, filePath, lineNumber);
        }

        public void LogWarning(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Warning(this.workflowContext.WorkflowId, message, this.workflowContext.UserUniqueId, memberName, filePath, lineNumber);
        }

        public void LogError(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Error(this.workflowContext.WorkflowId, message, this.workflowContext.UserUniqueId, memberName, filePath, lineNumber);
        }

        public void LogVerbose(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Verbose(this.workflowContext.WorkflowId, message, this.workflowContext.UserUniqueId, memberName, filePath, lineNumber);
        }


        public void LogException(string message, Exception ex, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.workflowContext.WorkflowId, ex, this.workflowContext.UserUniqueId, message, memberName, filePath, lineNumber);
        }

        public void LogException(Exception ex, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.workflowContext.WorkflowId, ex, this.workflowContext.UserUniqueId, null, memberName, filePath, lineNumber);
        }
    }
}
