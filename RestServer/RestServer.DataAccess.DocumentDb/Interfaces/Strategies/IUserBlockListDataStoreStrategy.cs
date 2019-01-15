using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.DocumentDb.Interfaces.Strategies
{
    public interface IUserBlockListDataStoreStrategy : IDocumentDbDataStoreStrategy<UserBlockList>
    {
        Task<UserBlockList> GetUserBlockList(int userId);

        Task<UserBlockList> InsertOrUpdateUserBlockList(UserBlockList userBlockList);
    }
}
