using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.Entities.DataAccess;
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

        IUserActivationRepository UserActivationRepository { get; }

        IGenericRepository<RestServerSetting> RestServerSettingRepository { get; } 

        IGenericRepository<UserSession> UserSessionRepository { get; }

        IApplicationRepository ApplicationRepository { get; }

        IGroupRepository GroupRepository { get; }

        Task<int> SaveAsync();
    }
}
