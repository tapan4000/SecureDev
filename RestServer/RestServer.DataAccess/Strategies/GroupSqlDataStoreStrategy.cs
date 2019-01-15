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
    public class GroupSqlDataStoreStrategy : DataStoreStrategyBase<Group>, IGroupDataStoreStrategy
    {
        public GroupSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {

        }
    }
}
