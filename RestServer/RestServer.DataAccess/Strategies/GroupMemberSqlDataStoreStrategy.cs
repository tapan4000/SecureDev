using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.Enums;

namespace RestServer.DataAccess.Strategies
{
    public class GroupMemberSqlDataStoreStrategy : DataStoreStrategyBase<GroupMember>, IGroupMemberDataStoreStrategy
    {
        public GroupMemberSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<int> GetActiveGroupCountByUserId(int userId)
        {
            return await this.GetCountByFilter(row => row.UserId == userId
            && (row.GroupMemberStateId == (int)GroupMemberStateEnum.Accepted
            || row.GroupMemberStateId == (int)GroupMemberStateEnum.PendingAcceptance
            || row.GroupMemberStateId == (int)GroupMemberStateEnum.PendingRequestForUpgradeToAdmin)).ConfigureAwait(false);
        }

        public async Task<int> GetActiveUserCountByGroupId(int groupId)
        {
            return await this.GetCountByFilter(row => row.GroupId == groupId 
            && (row.GroupMemberStateId == (int)GroupMemberStateEnum.Accepted
            || row.GroupMemberStateId == (int)GroupMemberStateEnum.PendingAcceptance
            || row.GroupMemberStateId == (int)GroupMemberStateEnum.PendingRequestForUpgradeToAdmin)).ConfigureAwait(false);
        }

        public async Task<bool> IsUserAlreadyAddedToGroup(int groupId, int userId)
        {
            return await this.IsRecordPresentByFilter(row => row.GroupId == groupId && row.UserId == userId).ConfigureAwait(false);
        }

        public async Task<bool> IsUserAlreadyHavingPrimaryGroup(int userId)
        {
            return await this.IsRecordPresentByFilter(row => row.UserId == userId && row.IsPrimary).ConfigureAwait(false);
        }

        public async Task<GroupMember> GetExistingGroupMemberRecord(int groupId, int userId)
        {
            var existingGroupMemberList = await this.GetData(row => row.GroupId == groupId && row.UserId == userId);
            if (existingGroupMemberList.Any())
            {
                return existingGroupMemberList.First();
            }

            return null;
        }
    }
}
