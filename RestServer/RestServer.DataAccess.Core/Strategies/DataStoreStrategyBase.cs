using RestServer.DataAccess.Core.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Strategies
{
    public class DataStoreStrategyBase<TEntity> : IDataStoreStrategy<TEntity> where TEntity : class
    {
        protected DataContextBase DataContext;

        private IDbSet<TEntity> dataSet;

        protected DataStoreStrategyBase(IDataContext dataContext)
        {
            this.DataContext = dataContext as DataContextBase;
            if(null != this.DataContext)
            {
                this.dataSet = this.DataContext.Set<TEntity>();
            }
        }

        public Task<bool> DeleteAsync(object id)
        {
            // TODO: Every delete operation against an id should run a delete stored procedure instead of fetching the record and then deleting it.
            throw new NotImplementedException();
        }

        public bool DeleteAsync(TEntity entityToDelete)
        {
            if(null != entityToDelete)
            {
                if(this.DataContext.Entry(entityToDelete).State == EntityState.Detached)
                {
                    this.dataSet.Attach(entityToDelete);
                }

                this.dataSet.Remove(entityToDelete);
            }

            return true;
        }

        public async Task<TEntity> GetById(object id)
        {
            try
            {
                this.DataContext.Configuration.AutoDetectChangesEnabled = false;
                var data = await Task.Run(() => this.dataSet.Find(id)).ConfigureAwait(false);
                return data;
            }
            finally
            {
                this.DataContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entityData = await Task.Run(() => this.dataSet.Add(entity)).ConfigureAwait(false);
            return entityData;
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            return Task.Run(() => this.UpdateEntity(entity));
        }

        protected Task<List<TEntity>> GetData(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = this.DataContext.GetQuery<TEntity>();
            if(null != filter)
            {
                query = query.Where(filter);
            }

            return query.ToListAsync();
        }

        private bool UpdateEntity(TEntity entity)
        {
            this.dataSet.Attach(entity);
            this.DataContext.Entry(entity).State = EntityState.Modified;
            return true;
        }

        protected async Task<int> GetCountByFilter(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = this.DataContext.GetQuery<TEntity>();
            return await Task.Run(() => query.Count(filter)).ConfigureAwait(false);
        }
    }
}
