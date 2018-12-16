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

namespace RestServer.DataAccess.Repositories
{
    public class UserActivationRepository : RepositoryBase<UserActivation>, IUserActivationRepository
    {
        public UserActivationRepository(IDependencyContainer dependencyContainer, IGenericDataStoreStrategy<UserActivation> dataStoreStrategy, ICacheStrategyHandler<UserActivation> cacheStrategyHandler, IEventLogger logger) 
            : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
        }
    }
}
