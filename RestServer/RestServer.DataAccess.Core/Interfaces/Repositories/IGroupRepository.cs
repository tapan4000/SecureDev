using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task SyncAnonymousGroupMemberRequests(int userId, string userIsdCode, string mobileNumber, int maxUserCountPerGroup, int maxGroupCountPerUser);
    }
}
