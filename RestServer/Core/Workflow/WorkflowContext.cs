namespace RestServer.Core.Workflow
{
    using System;

    using RestServer.Logging.Interfaces;
    using IoC;
    using Entities.DataAccess;

    [IoCRegistration(IoCLifetime.Hierarchical)]
    public class WorkflowContext : IWorkflowContext
    {
        public string WorkflowId { get; set; }

        public string ApplicationUniqueId { get; set; }

        public User User { get; set; }
    }
}
