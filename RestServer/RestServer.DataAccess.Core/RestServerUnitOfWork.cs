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
using RestServer.Entities.Interfaces;
using RestServer.Cache.Interfaces;

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

        private IGroupMemberRepository groupMemberRepository;

        private IGenericRepository<PublicGroup> publicGroupRepository;

        private IAnonymousGroupMemberRepository anonymousGroupMemberRepository;

        private ILocationCaptureSessionRepository locationCaptureSessionRepository;

        private IGenericRepository<MembershipTier> membershipTierRepository;

        private IConsolidatedCacheInvalidator cacheInvalidator;

        public RestServerUnitOfWork(IDependencyContainer dependencyContainer, IEventLogger logger, IUserContext userContext, IConsolidatedCacheInvalidator cacheInvalidator)
        {
            this.dependencyContainer = dependencyContainer.CreateChildContainer();
            this.logger = logger;
            this.userContext = userContext;

            // Necessary to resolve data context using child container, else the data context will be disposed after first usage and then will remain in disposed state.
            this.dataContext = this.dependencyContainer.Resolve<IDataContext>();
            this.cacheInvalidator = cacheInvalidator;
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

        public IGroupMemberRepository GroupMemberRepository
        {
            get
            {
                if(null == this.groupMemberRepository)
                {
                    this.groupMemberRepository = this.dependencyContainer.Resolve<IGroupMemberRepository>();
                }

                return this.groupMemberRepository;
            }
        }

        public IGenericRepository<PublicGroup> PublicGroupRepository
        {
            get
            {
                if(null == this.publicGroupRepository)
                {
                    this.publicGroupRepository = this.dependencyContainer.Resolve<IGenericRepository<PublicGroup>>();
                }

                return this.publicGroupRepository;
            }
        }

        public IAnonymousGroupMemberRepository AnonymousGroupMemberRepository
        {
            get
            {
                if(null == this.anonymousGroupMemberRepository)
                {
                    this.anonymousGroupMemberRepository = this.dependencyContainer.Resolve<IAnonymousGroupMemberRepository>();
                }

                return this.anonymousGroupMemberRepository;
            }
        }

        public ILocationCaptureSessionRepository LocationCaptureSessionRepository
        {
            get
            {
                if (null == this.locationCaptureSessionRepository)
                {
                    this.locationCaptureSessionRepository = this.dependencyContainer.Resolve<ILocationCaptureSessionRepository>();
                }

                return this.locationCaptureSessionRepository;
            }
        }

        public IGenericRepository<MembershipTier> MembershipTierRepository
        {
            get
            {
                if(null == this.membershipTierRepository)
                {
                    this.membershipTierRepository = this.dependencyContainer.Resolve<IGenericRepository<MembershipTier>>();
                }

                return this.membershipTierRepository;
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
                    modifiedEntity.CreatedBy = this.userContext.UserOrServiceIdentifier;
                }
                else
                {
                    modifiedEntity.LastModificationDateTime = currentUtcDateTime;
                    modifiedEntity.LastModifiedBy = this.userContext.UserOrServiceIdentifier;
                }
            }

            var result = await this.dataContext.SaveAsync().ConfigureAwait(false);
            await this.cacheInvalidator.invalidateAsync().ConfigureAwait(false);
            return result;
        }
    }
}
