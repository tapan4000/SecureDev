using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.DocumentDb.Interfaces.Strategies
{
    public interface IDocumentDbDataStoreStrategy<TEntity>
    {
        Task<DocumentDbRecord<TEntity>> GetByIdAndPartitionKey(object id, string partitionKey);

        Task<DocumentDbRecord<TEntity>> Insert(TEntity entity);

        Task<bool> Delete(object id);

        Task<DocumentDbRecord<TEntity>> UpsertAsync(TEntity entity);
    }
}
