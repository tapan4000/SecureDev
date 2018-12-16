namespace RestServer.Core.Workflow
{
    using System;

    using RestServer.Logging.Interfaces;
    using IoC;

    [IoCRegistration(IoCLifetime.Hierarchical)]
    public class WorkflowContext : IWorkflowContext
    {
        public string WorkflowId { get; set; }

        public string ApplicationUniqueId { get; set; }

        public int UserId { get; set; }

        public string UserUniqueId { get; set; }
    }
}
