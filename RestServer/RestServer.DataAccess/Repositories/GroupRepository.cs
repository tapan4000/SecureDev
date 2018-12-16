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
    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        private IGroupDataStoreStrategy dataStoreStrategy;

        public GroupRepository(IDependencyContainer dependencyContainer, IGroupDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<Group> cacheStrategyHandler, IEventLogger logger) : 
            base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.dataStoreStrategy = dataStoreStrategy;
        }

        public async Task<int> GetGroupCountByUserId(int userId)
        {
            return await this.dataStoreStrategy.GetGroupCountByUserId(userId).ConfigureAwait(false);
        }
    }
}
