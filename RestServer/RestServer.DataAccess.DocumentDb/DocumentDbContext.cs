using RestServer.Configuration.Models;
using RestServer.DataAccess.DocumentDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using RestServer.Entities.DataAccess.Core;
using RestServer.Core.Extensions;
using RestServer.IoC;
using RestServer.Entities;
using Microsoft.Azure.Documents.Client;

namespace RestServer.DataAccess.DocumentDb
{
    [IoCRegistration(IoCLifetime.ContainerControlled)]
    public class DocumentDbContext : IDocumentDbContext
    {
        private const string DocDbAccountEndpointPrefix = "AccountEndpoint=";

        private const string DocDbAccountKeyPrefix = "AccountKey=";

        private bool isDisposed = false;

        private string databaseName;

        private string userCollectionName;

        private IDocumentClient documentClient;

        public bool IsInitialized { get; set; }

        public string GetCollection(DocumentDbCollectionType collectionType)
        {
            switch (collectionType)
            {
                case DocumentDbCollectionType.User:
                    return this.userCollectionName;
                default:
                    throw new ArgumentException("Invalid collection type has been provided.");
            }
        }

        public string GetDatabase()
        {
            if (this.databaseName.IsEmpty())
            {
                throw new NullReferenceException("The database name cannot be empty.");
            }

            return this.databaseName;
        }

        public IDocumentClient GetDocumentClient()
        {
            if (null == this.documentClient)
            {
                throw new NullReferenceException("The document client cannot be null.");
            }

            return this.documentClient;
        }

        public IEnumerable<IEntityBase> GetModifiedEntities()
        {
            return default(IEnumerable<IEntityBase>);
        }

        public Task InitializeAsync(DocumentDbSetting docDbSetting, string docDbConnectionString)
        {
            if (docDbConnectionString.IsEmpty())
            {
                throw new ArgumentException(nameof(docDbConnectionString));
            }

            if (docDbSetting.DatabaseName.IsEmpty())
            {
                throw new ArgumentException(nameof(docDbSetting.DatabaseName));
            }

            if (docDbSetting.UserCollectionName.IsEmpty())
            {
                throw new ArgumentException(nameof(docDbSetting.UserCollectionName));
            }

            this.databaseName = docDbSetting.DatabaseName;
            this.userCollectionName = docDbSetting.UserCollectionName;

            var documentDbConnectionStringSplit = docDbConnectionString.Split(new[] { CoreConstants.SemiColonSeparator }, StringSplitOptions.None);
            var accountEndPoint = documentDbConnectionStringSplit.FirstOrDefault(t => t.Contains(DocDbAccountEndpointPrefix));
            var accountKey = documentDbConnectionStringSplit.FirstOrDefault(t => t.Contains(DocDbAccountKeyPrefix));
            if (accountEndPoint.IsEmpty() || accountKey.IsEmpty())
            {
                throw new Exception("Invalid Document Db Connection String.");
            }

            accountEndPoint = accountEndPoint.Replace(DocDbAccountEndpointPrefix, string.Empty);
            accountKey = accountKey.Replace(DocDbAccountKeyPrefix, string.Empty);

            var connectionPolicy = new ConnectionPolicy
            {
                RetryOptions = new RetryOptions
                {
                    MaxRetryWaitTimeInSeconds = docDbSetting.RetryIntervalInSeconds,
                    MaxRetryAttemptsOnThrottledRequests = docDbSetting.RetryCountOnThrottling
                },
                EnableEndpointDiscovery = true,
                MaxConnectionLimit = docDbSetting.MaxConnectionLimit,
                ConnectionProtocol = Protocol.Tcp,
                ConnectionMode = ConnectionMode.Direct
            };

            if (docDbSetting.MediaRequestTimeOutInSeconds != 0)
            {
                connectionPolicy.MediaRequestTimeout = TimeSpan.FromSeconds(docDbSetting.MediaRequestTimeOutInSeconds);
            }

            if (docDbSetting.RequestTimeoutInSeconds != 0)
            {
                connectionPolicy.RequestTimeout = TimeSpan.FromSeconds(docDbSetting.RequestTimeoutInSeconds);
            }

            this.documentClient = new DocumentClient(new Uri(accountEndPoint), accountKey, connectionPolicy, ConsistencyLevel.Session);
            this.IsInitialized = true;

            // Open the Document client at startup to establish TCP connection and avoid lag during actual call.
            var client = (DocumentClient)this.documentClient;
            return client.OpenAsync();
        }

        public Task<int> SaveAsync()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (!this.isDisposed)
            {
                if (isDisposing)
                {
                    if(null != this.documentClient)
                    {
                        this.documentClient = null;
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
