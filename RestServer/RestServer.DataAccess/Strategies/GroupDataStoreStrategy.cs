using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;

namespace RestServer.DataAccess.Strategies
{
    public class GroupDataStoreStrategy : DataStoreStrategyBase<Group>, IGroupDataStoreStrategy
    {
        public GroupDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {

        }

        public async Task<int> GetGroupCountByUserId(int userId)
        {
            var groupCount = await Task.Run(() => this.DataContext.GetDbSet<GroupMember>().Count(row => row.UserId == userId)).ConfigureAwait(false);
            return groupCount;
        }
    }
}
