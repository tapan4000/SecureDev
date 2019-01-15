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
    public class AnonymousGroupMemberRepository : RepositoryBase<AnonymousGroupMember>, IAnonymousGroupMemberRepository
    {
        private IAnonymousGroupMemberDataStoreStrategy dataStoreStrategy;

        public AnonymousGroupMemberRepository(IDependencyContainer dependencyContainer, IAnonymousGroupMemberDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<AnonymousGroupMember> cacheStrategyHandler, IEventLogger logger) : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.dataStoreStrategy = dataStoreStrategy;
        }

        public Task<AnonymousGroupMember> GetExistingAnonymousGroupMemberRecord(string isdCode, string mobileNumber, int groupId)
        {
            return this.dataStoreStrategy.GetExistingAnonymousGroupMemberRecord(isdCode, mobileNumber, groupId);
        }
    }
}
