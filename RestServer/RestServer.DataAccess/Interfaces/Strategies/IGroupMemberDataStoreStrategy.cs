using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Interfaces.Strategies
{
    public interface IGroupMemberDataStoreStrategy : IDataStoreStrategy<GroupMember>
    {
        Task<int> GetActiveGroupCountByUserId(int userId);

        Task<int> GetActiveUserCountByGroupId(int groupId);

        Task<bool> IsUserAlreadyAddedToGroup(int groupId, int userId);

        Task<bool> IsUserAlreadyHavingPrimaryGroup(int userId);

        Task<GroupMember> GetExistingGroupMemberRecord(int groupId, int userId);
    }
}
