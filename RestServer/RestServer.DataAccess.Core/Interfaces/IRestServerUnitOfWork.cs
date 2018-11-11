using RestServer.DataAccess.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces
{
    public interface IRestServerUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        Task<int> SaveAsync();
    }
}
