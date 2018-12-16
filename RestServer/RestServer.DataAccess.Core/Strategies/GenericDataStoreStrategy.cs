using RestServer.DataAccess.Core.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;

namespace RestServer.DataAccess.Core.Strategies
{
    public class GenericDataStoreStrategy<TEntity> : DataStoreStrategyBase<TEntity>, IGenericDataStoreStrategy<TEntity> where TEntity : class
    {
        public GenericDataStoreStrategy(IDataContext dataContext) : base(dataContext)
        {
        }
    }
}
