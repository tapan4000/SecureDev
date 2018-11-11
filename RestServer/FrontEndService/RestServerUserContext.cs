using RestServer.DataAccess.Core.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService
{
    public class RestServerUserContext : IUserContext
    {
        private readonly IWorkflowContext workflowContext;

        private const string RestServerName = "RestServer";

        public RestServerUserContext(IWorkflowContext workflowContext)
        {
            this.workflowContext = workflowContext;
        }

        public string UserName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.workflowContext.UserUniqueId) ? RestServerName : this.workflowContext.UserUniqueId;
            }
        }
    }
}
