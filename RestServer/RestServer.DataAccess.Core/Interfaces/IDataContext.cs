using RestServer.Entities.DataAccess;
using RestServer.Entities.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces
{
    public interface IDataContext
    {
        Task<int> SaveAsync();

        IEnumerable<IEntityBase> GetModifiedEntities();

        void Dispose();
    }
}
