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
    public class ApplicationSqlDataStoreStrategy : DataStoreStrategyBase<Application>, IApplicationDataStoreStrategy
    {
        public ApplicationSqlDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public async Task<Application> GetApplicationByUniqueId(string appUniqueId)
        {
            var application = (await this.GetData(appObj => appObj.ApplicationUniqueId.Equals(appUniqueId, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false)).FirstOrDefault();
            return application;
        }
    }
}
