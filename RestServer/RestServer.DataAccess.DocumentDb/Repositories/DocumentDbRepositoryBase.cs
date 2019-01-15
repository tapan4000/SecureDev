using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;
using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Models;

namespace RestServer.DataAccess.DocumentDb.Repositories
{
    public abstract class DocumentDbRepositoryBase<TEntity> : IDocumentDbRepository<TEntity>
    {
        private readonly IDocumentDbDataStoreStrategy<TEntity> documentDbDataStoreStrategy;

        public DocumentDbRepositoryBase(IDocumentDbDataStoreStrategy<TEntity> documentDbDataStoreStrategy)
        {
            this.documentDbDataStoreStrategy = documentDbDataStoreStrategy;
        }

        public Task<bool> DeleteAsync(object id)
        {
            return this.documentDbDataStoreStrategy.Delete(id);
        }

        public Task<DocumentDbRecord<TEntity>> GetByIdAndPartitionKeyAsync(object id, string partitionKey)
        {
            return this.documentDbDataStoreStrategy.GetByIdAndPartitionKey(id, partitionKey);
        }

        public Task<DocumentDbRecord<TEntity>> InsertDocument(TEntity entity)
        {
            return this.documentDbDataStoreStrategy.Insert(entity);
        }

        public Task<DocumentDbRecord<TEntity>> UpsertAsync(TEntity entity)
        {
            return this.documentDbDataStoreStrategy.UpsertAsync(entity);
        }
    }
}
