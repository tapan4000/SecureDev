using RestServer.Cache;
using RestServer.Cache.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.Entities;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Repositories
{
    public class RepositoryBase<TEntity> : IRepository<TEntity>
    {
        private readonly IDependencyContainer dependencyContainer;

        protected readonly IDataStoreStrategy<TEntity> dataStoreStrategy;

        protected readonly ICacheStrategyHandler<TEntity> cacheStrategyHandler;

        private IEventLogger logger;

        public RepositoryBase(IDependencyContainer dependencyContainer, IDataStoreStrategy<TEntity> dataStoreStrategy, ICacheStrategyHandler<TEntity> cacheStrategyHandler, IEventLogger logger)
        {
            this.dependencyContainer = dependencyContainer;
            this.dataStoreStrategy = dataStoreStrategy;
            this.cacheStrategyHandler = cacheStrategyHandler;
            this.logger = logger;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            if(null == id)
            {
                return true;
            }

            var result = await this.cacheStrategyHandler.DeleteFromStoreAsync(id.ToString()).ConfigureAwait(false);

            if (result)
            {
                result = await this.dataStoreStrategy.DeleteAsync(id).ConfigureAwait(false);
            }
            else
            {
                this.logger.LogError($"Failed to delete from cache: {id}");
            }

            return result;
        }

        public async Task<TEntity> GetById(object id)
        {
            if(null == id)
            {
                return default(TEntity);
            }

            var cachedEntity = await this.cacheStrategyHandler.GetFromStoreAsync(id.ToString()).ConfigureAwait(false);
            if(null != cachedEntity)
            {
                return cachedEntity;
            }

            var entityValue = await this.dataStoreStrategy.GetById(id).ConfigureAwait(false);
            if(null != entityValue)
            {
                await this.cacheStrategyHandler.InsertOrUpdateInStoreAsync(id.ToString(), entityValue).ConfigureAwait(false);
            }

            return entityValue;
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            return this.dataStoreStrategy.InsertAsync(entity);
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            var result = await this.cacheStrategyHandler.DeleteFromStoreAsync(CacheTypeToKeyPropertyMap.GetFinalKeyListForType(entity)).ConfigureAwait(false);

            if (result)
            {
                result = await this.dataStoreStrategy.UpdateAsync(entity).ConfigureAwait(false);
            }
            else
            {
                this.logger.LogError($"Failed to update the cache for entity: {typeof(TEntity).FullName}");
            }

            return result;
        }
    }
}
