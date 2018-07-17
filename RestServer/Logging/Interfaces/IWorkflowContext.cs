namespace RestServer.Logging.Interfaces
{
    using System;

    public interface IWorkflowContext
    {
        Guid WorkflowId { get; set; }

        string UserUniqueId { get; set; }
    }
}
