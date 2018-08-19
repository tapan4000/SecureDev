namespace RestServer.Logging.Interfaces
{
    using System;

    public interface IWorkflowContext
    {
        string WorkflowId { get; set; }

        string UserUniqueId { get; set; }
    }
}
