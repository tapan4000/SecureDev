using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Strategies
{
    public interface IDataStoreStrategy<TEntity>
    {
        Task<TEntity> GetById(object id);

        Task<bool> DeleteAsync(object id);

        Task<bool> InsertAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);
    }
}
