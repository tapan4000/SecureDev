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
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Logging.Interfaces;

namespace RestServer.DataAccess.Repositories
{
    public class ApplicationRepository : RepositoryBase<Application>, IApplicationRepository
    {
        private IApplicationDataStoreStrategy appDataStoreStrategy;

        public ApplicationRepository(IDependencyContainer dependencyContainer, IApplicationDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<Application> cacheStrategyHandler, IEventLogger logger) 
            : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.appDataStoreStrategy = dataStoreStrategy;
        }

        public Task<Application> GetApplicationByUniqueId(string appUniqueId)
        {
            return this.appDataStoreStrategy.GetApplicationByUniqueId(appUniqueId);
        }
    }
}
