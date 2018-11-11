using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.DataAccess.Interfaces.Strategies;

namespace RestServer.DataAccess.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private IUserDataStoreStrategy dataStoreStrategy;

        public UserRepository(IDependencyContainer dependencyContainer, IUserDataStoreStrategy dataStoreStrategy) : base(dependencyContainer, dataStoreStrategy)
        {
            this.dataStoreStrategy = dataStoreStrategy;
        }

        public async Task<User> GetUserByMobileNumber(string mobileNumber)
        {
            var user = await this.dataStoreStrategy.GetUserByMobileNumber(mobileNumber).ConfigureAwait(false);
            return user;
        }
    }
}
