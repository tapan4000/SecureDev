using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Strategies
{
    public interface IGenericDataStoreStrategy<TEntity> : IDataStoreStrategy<TEntity>
    {
    }
}
