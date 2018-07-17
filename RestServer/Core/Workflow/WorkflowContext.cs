namespace RestServer.Core.Workflow
{
    using System;

    using RestServer.Logging.Interfaces;

    public class WorkflowContext : IWorkflowContext
    {
        public Guid WorkflowId { get; set; }

        public string UserUniqueId { get; set; }
    }
}
