using RestServer.DataAccess.Core.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Strategies
{
    public class DataStoreStrategyBase<TEntity> : IDataStoreStrategy<TEntity>
    {
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
