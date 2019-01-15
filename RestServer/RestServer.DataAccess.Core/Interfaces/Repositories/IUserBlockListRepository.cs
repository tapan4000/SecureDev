using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface IUserBlockListRepository : IDocumentDbRepository<UserBlockList>
    {
        Task<UserBlockList> GetUserBlockList(int userId);

        Task<UserBlockList> InsertOrUpdateUserBlockList(UserBlockList blockList);
    }
}
