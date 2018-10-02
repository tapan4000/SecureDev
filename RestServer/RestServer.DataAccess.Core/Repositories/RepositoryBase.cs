using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    {
        private readonly IDependencyContainer dependencyContainer;

        private readonly IDataStoreStrategy<TEntity> dataStoreStrategy;

        public RepositoryBase(IDependencyContainer dependencyContainer, IDataStoreStrategy<TEntity> dataStoreStrategy)
        {
            this.dependencyContainer = dependencyContainer;
            this.dataStoreStrategy = dataStoreStrategy;
        }

        public Task<bool> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
