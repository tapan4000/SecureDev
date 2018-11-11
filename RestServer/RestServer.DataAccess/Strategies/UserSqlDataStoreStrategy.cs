using RestServer.DataAccess.Core.Interfaces;
using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Strategies
{
    public class UserSqlDataStoreStrategy : DataStoreStrategyBase<User>, IUserDataStoreStrategy
    {
        public UserSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<User> GetUserByMobileNumber(string mobileNumber)
        {
            var user = (await this.GetData(userObj => userObj.MobileNumber.Equals(mobileNumber, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false)).FirstOrDefault();
            return user;
        }
    }
}
