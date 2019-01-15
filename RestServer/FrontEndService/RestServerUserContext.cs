using RestServer.Core.Extensions;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.DataAccess;
using RestServer.Entities.Interfaces;
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

        public string UserOrServiceIdentifier
        {
            get
            {
                return null == this.workflowContext.User ? RestServerName : this.workflowContext.User.UserId.ToString();
            }
        }

        public string WorkflowId
        {
            get
            {
                if (this.workflowContext.WorkflowId.IsEmpty())
                {
                    var workflowId = Guid.NewGuid().ToString();
                    this.workflowContext.WorkflowId = workflowId;
                }

                return this.workflowContext.WorkflowId;
            }
        }

        public int UserId
        {
            get
            {
                if(null != this.workflowContext.User)
                {
                    return this.workflowContext.User.UserId;
                }

                return 0;
            }
        }

        public string ApplicationUniqueId
        {
            get
            {
                if (!this.workflowContext.ApplicationUniqueId.IsEmpty())
                {
                    return this.workflowContext.ApplicationUniqueId;
                }

                return null;
            }
        }

        public User User
        {
            get
            {
                return this.workflowContext.User;
            }
        }
    }
}
