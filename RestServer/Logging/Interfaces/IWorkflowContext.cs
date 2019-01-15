namespace RestServer.Logging.Interfaces
{
    using Entities.DataAccess;
    using System;

    public interface IWorkflowContext
    {
        string WorkflowId { get; set; }

        string ApplicationUniqueId { get; set; }

        User User { get; set; }
    }
}
