using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;
using RestServer.DataAccess.Core.Models;
using RestServer.DataAccess.Core.Interfaces.Repositories;

namespace RestServer.DataAccess.DocumentDb.Repositories
{
    public class UserBlockListRepository : DocumentDbRepositoryBase<UserBlockList>, IUserBlockListRepository
    {
        private IUserBlockListDataStoreStrategy userBlockListDataStoreStrategy;

        public UserBlockListRepository(IUserBlockListDataStoreStrategy documentDbDataStoreStrategy) : base(documentDbDataStoreStrategy)
        {
            this.userBlockListDataStoreStrategy = documentDbDataStoreStrategy;
        }

        public async Task<UserBlockList> GetUserBlockList(int userId)
        {
            var userBlockListDocument = await this.userBlockListDataStoreStrategy.GetUserBlockList(userId);
            return userBlockListDocument;
        }

        public async Task<UserBlockList> InsertOrUpdateUserBlockList(UserBlockList blockList)
        {
            return await this.userBlockListDataStoreStrategy.InsertOrUpdateUserBlockList(blockList);
        }
    }
}
