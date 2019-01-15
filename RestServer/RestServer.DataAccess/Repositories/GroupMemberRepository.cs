using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Cache.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Interfaces.Strategies;

namespace RestServer.DataAccess.Repositories
{
    public class GroupMemberRepository : RepositoryBase<GroupMember>, IGroupMemberRepository
    {
        private IGroupMemberDataStoreStrategy groupMemberDataStoreStrategy;

        public GroupMemberRepository(IDependencyContainer dependencyContainer, IGroupMemberDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<GroupMember> cacheStrategyHandler, IEventLogger logger) 
            : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.groupMemberDataStoreStrategy = dataStoreStrategy;
        }

        public async Task<int> GetActiveGroupCountByUserId(int userId)
        {
            return await this.groupMemberDataStoreStrategy.GetActiveGroupCountByUserId(userId).ConfigureAwait(false);
        }

        public async Task<int> GetActiveUserCountByGroupId(int groupId)
        {
            return await this.groupMemberDataStoreStrategy.GetActiveUserCountByGroupId(groupId).ConfigureAwait(false);
        }

        public async Task<bool> IsUserAlreadyAddedToGroup(int groupId, int userId)
        {
            return await this.groupMemberDataStoreStrategy.IsUserAlreadyAddedToGroup(groupId, userId).ConfigureAwait(false);
        }

        public async Task<bool> IsUserAlreadyHavingPrimaryGroup(int userId)
        {
            return await this.groupMemberDataStoreStrategy.IsUserAlreadyHavingPrimaryGroup(userId).ConfigureAwait(false);
        }

        public async Task<GroupMember> GetExistingGroupMemberRecord(int groupId, int userId)
        {
            return await this.groupMemberDataStoreStrategy.GetExistingGroupMemberRecord(groupId, userId).ConfigureAwait(false);
        }
    }
}
