using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Interfaces
{
    public interface IUserContext
    {
        int UserId { get; }

        string UserOrServiceIdentifier { get; }

        string WorkflowId { get; }

        string ApplicationUniqueId { get; }

        User User { get; }
    }
}
