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

namespace RestServer.DataAccess.Core
{
    public class RestServerUnitOfWork : IRestServerUnitOfWork
    {
        private IDependencyContainer dependencyContainer;
        private IDataContext dataContext;
        private IEventLogger logger;
        private IUserContext userContext;

        private IUserRepository userRepository;

        public RestServerUnitOfWork(IDependencyContainer dependencyContainer, IEventLogger logger, IUserContext userContext)
        {
            this.dependencyContainer = dependencyContainer;
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(null != this.dataContext)
                {
                    this.dataContext.Dispose();
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
                    if(null == modifiedEntity.CreationDateTime)
                    {
                        modifiedEntity.CreationDateTime = currentUtcDateTime;
                    }

                    modifiedEntity.CreatedBy = this.userContext.UserName;
                }

                modifiedEntity.LastModificationDateTime = currentUtcDateTime;
                modifiedEntity.LastModifiedBy = this.userContext.UserName;
            }

            return await this.dataContext.SaveAsync().ConfigureAwait(false);
        }
    }
}
