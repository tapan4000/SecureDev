namespace RestServer.Logging
{
    using System;
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
            this.WriteDebugMessage(message);
        }

        public void LogWarning(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            this.WriteDebugMessage(message);
        }

        public void LogError(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            this.WriteDebugMessage(message);
        }

        public void LogVerbose(string message, string filePath = "", int lineNumber = 0, string memberName = "")
        {
            this.WriteDebugMessage(message);
        }

        private void WriteDebugMessage(string message)
        {
            LoggerEventSource.Current.Info(this.workflowContext.WorkflowId.ToString(), message);
        }

        public void LogException(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            this.WriteDebugMessage(this.FlattenException(ex) + message);
        }

        private string FlattenException(Exception ex)
        {
            if (null == ex.InnerException)
            {
                return ex.Message;
            }
            else
            {
                StringBuilder exceptionMessage = new StringBuilder(ex.Message);
                var innerException = ex.InnerException;
                while (null != innerException)
                {
                    exceptionMessage.Append("---------");
                    exceptionMessage.Append(innerException.Message);
                    innerException = innerException.InnerException;
                }

                return exceptionMessage.ToString();
            }
        }
    }
}
