using Microsoft.Azure.Documents;
using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Core.Models;
using RestServer.Entities.Interfaces;
using RestServer.DataAccess.Core.Helpers;

namespace RestServer.DataAccess.DocumentDb.Strategies
{
    public class UserBlockListDocumentDbDataStoreStrategy : DocumentDbDataStoreStrategyBase<UserBlockList>, IUserBlockListDataStoreStrategy
    {
        public UserBlockListDocumentDbDataStoreStrategy(IDocumentDbContext documentDbContext, IUserContext userContext, IEventLogger logger) 
            : base(documentDbContext, documentDbContext.GetCollection(DocumentDbCollectionType.User), userContext, logger)
        {
        }

        public async Task<UserBlockList> GetUserBlockList(int userId)
        {
            try
            {
                var userBlockList = await this.GetByIdAndPartitionKey(DocumentDbHelper.GetUserBlockListDocumentId(userId), DocumentDbHelper.GetUserDocumentPartitionKey(userId)).ConfigureAwait(false);
                userBlockList.Entity.LastModificationDateTime = userBlockList.LastModificationDateTime;
                return userBlockList.Entity;
            }
            catch(DocumentClientException ex)
            {
                if(ex.Error.Code.Equals(DocumentDbConstants.DocumentNotFoundErrorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<UserBlockList> InsertOrUpdateUserBlockList(UserBlockList userBlockList)
        {
            var insertedOrUpdatedDocument = await this.UpsertAsync(userBlockList);
            if(null != insertedOrUpdatedDocument)
            {
                return insertedOrUpdatedDocument.Entity;
            }

            return null;
        }
    }
}
