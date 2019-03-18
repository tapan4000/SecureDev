using RestServer.DataAccess.Core.Models;
using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces;
using RestServer.Entities.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Core.Helpers;
using Microsoft.Azure.Documents;

namespace RestServer.DataAccess.DocumentDb.Strategies
{
    public class UserLocationDocumentDbDataStoreStrategy : DocumentDbDataStoreStrategyBase<UserLocation>, IUserLocationDataStoreStrategy
    {
        public UserLocationDocumentDbDataStoreStrategy(IDocumentDbContext documentDbContext, IUserContext userContext, IEventLogger logger) : base(documentDbContext, documentDbContext.GetCollection(DocumentDbCollectionType.User), userContext, logger)
        {
        }

        public async Task<UserLocation> GetUserLocation(int userId)
        {
            try
            {
                var userLocation = await this.GetByIdAndPartitionKey(DocumentDbHelper.GetUserLocationDocumentId(userId), DocumentDbHelper.GetUserDocumentPartitionKey(userId)).ConfigureAwait(false);
                userLocation.Entity.LastModificationDateTime = userLocation.LastModificationDateTime;
                return userLocation.Entity;
            }
            catch (DocumentClientException ex)
            {
                if (ex.Error.Code.Equals(DocumentDbConstants.DocumentNotFoundErrorCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<UserLocation> InsertOrUpdateUserLocation(UserLocation userLocation)
        {
            var insertedOrUpdatedDocument = await this.UpsertAsync(userLocation);
            if (null != insertedOrUpdatedDocument)
            {
                return insertedOrUpdatedDocument.Entity;
            }

            return null;
        }
    }
}
