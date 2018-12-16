using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Interfaces.Strategies
{
    public interface IApplicationDataStoreStrategy : IDataStoreStrategy<Application>
    {
        Task<Application> GetApplicationByUniqueId(string appUniqueId);
    }
}
