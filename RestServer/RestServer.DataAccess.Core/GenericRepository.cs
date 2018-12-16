using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Cache.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;

namespace RestServer.DataAccess.Core
{
    public class GenericRepository<TEntity> : RepositoryBase<TEntity>, IGenericRepository<TEntity>
    {
        public GenericRepository(IDependencyContainer dependencyContainer, IGenericDataStoreStrategy<TEntity> dataStoreStrategy, ICacheStrategyHandler<TEntity> cacheStrategyHandler, IEventLogger logger) 
        : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
        }
    }
}
