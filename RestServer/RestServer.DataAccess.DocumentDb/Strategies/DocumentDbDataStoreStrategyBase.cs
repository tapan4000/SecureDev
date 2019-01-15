using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Net;
using Newtonsoft.Json;
using RestServer.Core.Extensions;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.DataAccess.Core.Models;
using RestServer.Entities.Interfaces;

namespace RestServer.DataAccess.DocumentDb.Strategies
{
    public abstract class DocumentDbDataStoreStrategyBase<TEntity> : IDocumentDbDataStoreStrategy<TEntity> where TEntity : IDocumentDbEntity
    {
        private readonly IDocumentClient documentClient;

        private readonly IUserContext userContext;

        private readonly IEventLogger logger;

        private readonly string collection;

        private readonly string database;

        public DocumentDbDataStoreStrategyBase(IDocumentDbContext documentDbContext, string collection, IUserContext userContext, IEventLogger logger)
        {
            this.collection = collection;
            this.database = documentDbContext.GetDatabase();
            this.documentClient = documentDbContext.GetDocumentClient();
            this.userContext = userContext;
            this.logger = logger;
        }

        public async Task<DocumentDbRecord<TEntity>> GetByIdAndPartitionKey(object id, string partitionKey)
        {
            var documentUri = this.GetDocumentUri(id);
            ResourceResponse<Document> response;

            if (partitionKey.IsEmpty())
            {
                response = await this.documentClient.ReadDocumentAsync(documentUri).ConfigureAwait(false);
            }
            else
            {
                var requestOptions = new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                };

                response = await this.documentClient.ReadDocumentAsync(documentUri, requestOptions).ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return this.GetDocumentDbRecord(response);
            }

            throw new DocumentException(response.StatusCode);
        }

        public async Task<DocumentDbRecord<TEntity>> Insert(TEntity entity)
        {
            if (entity.Id.IsEmpty())
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            return await this.Insert(entity, entity.PartitionKey);
        }

        private async Task<DocumentDbRecord<TEntity>> Insert(TEntity entity, string partitionKey)
        {
            this.SetEntityCreateUpdateParameters(entity);

            var documentCollectionUri = this.GetDocumentCollectionUri();
            ResourceResponse<Document> response = null;
            if (partitionKey.IsEmpty())
            {
                response = await this.documentClient.CreateDocumentAsync(documentCollectionUri, entity).ConfigureAwait(false);
            }
            else
            {
                var requestOptions = new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                };

                response = await this.documentClient.CreateDocumentAsync(documentCollectionUri, entity, requestOptions);
            }

            if(response.StatusCode == HttpStatusCode.Created)
            {
                return this.GetDocumentDbRecord(response);
            }

            throw new DocumentException(response.StatusCode);
        }
        public async Task<bool> Delete(object id)
        {
            var documentUri = this.GetDocumentUri(id);
            var response = await this.documentClient.DeleteDocumentAsync(documentUri).ConfigureAwait(false);

            if(response.StatusCode != HttpStatusCode.NotFound)
            {
                this.logger.LogError($"Failed to delete the document with id {id}.");
                return false;
            }

            return true;
        }

        public async Task<DocumentDbRecord<TEntity>> UpsertAsync(TEntity entity)
        {
            if (entity.Id.IsEmpty())
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            return await this.UpsertAsync(entity, entity.PartitionKey);
        }

        private async Task<DocumentDbRecord<TEntity>> UpsertAsync(TEntity entity, string partitionKey)
        {
            this.SetEntityCreateUpdateParameters(entity);
            var documentCollectionUri = this.GetDocumentCollectionUri();
            ResourceResponse<Document> response;
            if (partitionKey.IsEmpty())
            {
                response = await this.documentClient.UpsertDocumentAsync(documentCollectionUri, entity).ConfigureAwait(false);
            }
            else
            {
                var requestOptions = new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                };

                response = await this.documentClient.UpsertDocumentAsync(documentCollectionUri, entity, requestOptions).ConfigureAwait(false);
            }

            if(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return this.GetDocumentDbRecord(response);
            }

            throw new DocumentException(response.StatusCode);
        }

        private void SetEntityCreateUpdateParameters(TEntity entity)
        {
            if (null == entity.CreationDateTime)
            {
                entity.CreationDateTime = DateTime.UtcNow;
                entity.CreatedBy = this.userContext.UserOrServiceIdentifier;
            }

            // Not updating the last modified date as the timestamp is used to determine the last modification date/time for the document.
            entity.LastModifiedBy = this.userContext.UserOrServiceIdentifier;
        }

        private Uri GetDocumentUri(object id)
        {
            return UriFactory.CreateDocumentUri(this.database, this.collection, id.ToString());
        }

        private Uri GetDocumentCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(this.database, this.collection);
        }

        private DocumentDbRecord<TEntity> GetDocumentDbRecord(ResourceResponse<Document> response)
        {
            var deserializedEntity = JsonConvert.DeserializeObject<TEntity>(response.Resource.ToString());
            deserializedEntity.LastModificationDateTime = response.Resource.Timestamp;
            var documentDbRecord = new DocumentDbRecord<TEntity>
            {
                Entity = deserializedEntity,
                LastModificationDateTime = response.Resource.Timestamp,
                TimeToLive = response.Resource.TimeToLive
            };

            return documentDbRecord;
        }
    }
}
