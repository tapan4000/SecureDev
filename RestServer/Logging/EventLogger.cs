﻿namespace RestServer.Logging
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;
    using RestServer.Logging.Interfaces;
    using System.Text;
    using System.Threading.Tasks;
    using ServerContext;
    using System.Diagnostics;

    public class EventLogger : IEventLogger
    {
        private readonly IWorkflowContext workflowContext;

        public EventLogger(IWorkflowContext workflowContext)
        {
            this.workflowContext = workflowContext;
        }

        public void LogInformation(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Info(this.workflowContext.WorkflowId, message, this.workflowContext.UserId, memberName, fileName, lineNumber);
        }

        public void LogWarning(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Warning(this.workflowContext.WorkflowId, message, this.workflowContext.UserId, memberName, fileName, lineNumber);
        }

        public void LogError(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Error(this.workflowContext.WorkflowId, message, this.workflowContext.UserId, memberName, fileName, lineNumber);
        }

        public void LogVerbose(string message, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Verbose(this.workflowContext.WorkflowId, message, this.workflowContext.UserId, memberName, fileName, lineNumber);
        }


        public void LogException(string message, Exception ex, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.workflowContext.WorkflowId, ex, this.workflowContext.UserId, message, memberName, fileName, lineNumber);
        }

        public void LogException(Exception ex, string fileName = "", int lineNumber = 0, string memberName = "")
        {
            LoggerEventSource.Current.Critical(this.workflowContext.WorkflowId, ex, this.workflowContext.UserId, null, memberName, fileName, lineNumber);
        }
    }
}
