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
using RestServer.DataAccess.Interfaces.StoredProcedureAccessStrategies;
using RestServer.Cache;

namespace RestServer.DataAccess.Repositories
{
    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        private IGroupDataStoreStrategy groupDataStoreStrategy;

        private IGroupStoredProcedureAccessStrategy storedProcedureAccessStrategy;

        private ICacheStrategyHandler<User> userBasedCacheStrategyHandler;

        public GroupRepository(IDependencyContainer dependencyContainer, IGroupDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<User> userBasedCacheStrategyHandler, ICacheStrategyHandler<Group> cacheStrategyHandler, IEventLogger logger, IGroupStoredProcedureAccessStrategy storedProcedureAccessStrategy) : 
            base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.groupDataStoreStrategy = dataStoreStrategy;
            this.storedProcedureAccessStrategy = storedProcedureAccessStrategy;
            this.userBasedCacheStrategyHandler = userBasedCacheStrategyHandler;
        }

        public Task SyncAnonymousGroupMemberRequests(int userId, string userIsdCode, string mobileNumber, int maxUserCountPerGroup, int maxGroupCountPerUser)
        {
            // Clear the user's entry in REDIS cache as the User record will be updated with the flag for AnonyousGroupMemberVerified.
            this.userBasedCacheStrategyHandler.DeleteFromStoreAsync(CacheTypeToKeyPropertyMap.GetUserBasedCacheFinalKeys(userId, userIsdCode, mobileNumber));
            return this.storedProcedureAccessStrategy.SyncAnonymousGroupMemberRequests(userId, userIsdCode, mobileNumber, maxUserCountPerGroup, maxGroupCountPerUser);
        }
    }
}
