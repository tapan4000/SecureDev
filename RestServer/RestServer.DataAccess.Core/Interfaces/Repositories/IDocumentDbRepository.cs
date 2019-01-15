using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface IDocumentDbRepository<TEntity>
    {
        Task<DocumentDbRecord<TEntity>> GetByIdAndPartitionKeyAsync(object id, string partitionKey);

        Task<DocumentDbRecord<TEntity>> UpsertAsync(TEntity entity);

        Task<DocumentDbRecord<TEntity>> InsertDocument(TEntity entity);

        Task<bool> DeleteAsync(object id);
    }
}
