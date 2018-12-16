using RestServer.DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.Entities.DataAccess.Core;
using RestServer.Entities.DataAccess;

namespace RestServer.DataAccess.Core
{
    public class RestServerUnitOfWork : IRestServerUnitOfWork
    {
        private IDependencyContainer dependencyContainer;
        private IDataContext dataContext;
        private IEventLogger logger;
        private IUserContext userContext;

        private IUserRepository userRepository;

        private IUserActivationRepository userActivationRepository;

        private IGenericRepository<RestServerSetting> restServerSettingRepository;

        private IGenericRepository<UserSession> userSessionRepository;

        private IApplicationRepository applicationRepository;

        private IGroupRepository groupRepository;

        public RestServerUnitOfWork(IDependencyContainer dependencyContainer, IEventLogger logger, IUserContext userContext)
        {
            this.dependencyContainer = dependencyContainer.CreateChildContainer();
            this.logger = logger;
            this.userContext = userContext;
            this.dataContext = this.dependencyContainer.Resolve<IDataContext>();
        }

        public IUserRepository UserRepository
        {
            get
            {
                if(null == this.userRepository)
                {
                    this.userRepository = this.dependencyContainer.Resolve<IUserRepository>();
                }

                return this.userRepository;
            }
        }

        public IUserActivationRepository UserActivationRepository
        {
            get
            {
                if(null == this.userActivationRepository)
                {
                    this.userActivationRepository = this.dependencyContainer.Resolve<IUserActivationRepository>();
                }

                return this.userActivationRepository;
            }
        }

        public IGenericRepository<RestServerSetting> RestServerSettingRepository
        {
            get
            {
                if (null == this.restServerSettingRepository)
                {
                    this.restServerSettingRepository = this.dependencyContainer.Resolve<IGenericRepository<RestServerSetting>>();
                }

                return this.restServerSettingRepository;
            }
        }

        public IGenericRepository<UserSession> UserSessionRepository
        {
            get
            {
                if(null == this.userSessionRepository)
                {
                    this.userSessionRepository = this.dependencyContainer.Resolve<IGenericRepository<UserSession>>();
                }

                return this.userSessionRepository;
            }
        }

        public IApplicationRepository ApplicationRepository
        {
            get
            {
                if(null == this.applicationRepository)
                {
                    this.applicationRepository = this.dependencyContainer.Resolve<IApplicationRepository>();
                }

                return this.applicationRepository;
            }
        }

        public IGroupRepository GroupRepository
        {
            get
            {
                if(null == this.groupRepository)
                {
                    this.groupRepository = this.dependencyContainer.Resolve<IGroupRepository>();
                }

                return this.groupRepository;
            }
        }

        public void Dispose()  
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this.dataContext)
                {
                    this.dataContext.Dispose();
                }

                if(null != this.dependencyContainer)
                {
                    this.dependencyContainer.Dispose();
                }
            }
        }

        public async Task<int> SaveAsync()
        {
            var modifiedEntities = this.dataContext.GetModifiedEntities();
            foreach(var modifiedEntity in modifiedEntities)
            {
                var currentUtcDateTime = DateTime.UtcNow;
                if(modifiedEntity.ObjectState == ObjectState.Added)
                {
                    modifiedEntity.CreationDateTime = currentUtcDateTime;
                    modifiedEntity.CreatedBy = this.userContext.UserName;
                }
                else
                {
                    modifiedEntity.LastModificationDateTime = currentUtcDateTime;
                    modifiedEntity.LastModifiedBy = this.userContext.UserName;
                }
            }

            return await this.dataContext.SaveAsync().ConfigureAwait(false);
        }
    }
}
