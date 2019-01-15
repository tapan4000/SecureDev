using RestServer.DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.IoC.Interfaces;

namespace RestServer.DataAccess.DocumentDb
{
    public class DocumentDbUnitOfWork : IDocumentDbUnitOfWork
    {
        private IUserBlockListRepository userBlockListRepository;

        private IUserLocationRepository userLocationRepository;

        private IDependencyContainer dependencyContainer;

        public DocumentDbUnitOfWork(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IUserBlockListRepository UserBlockListRepository
        {
            get
            {
                if(null == this.userBlockListRepository)
                {
                    this.userBlockListRepository = this.dependencyContainer.Resolve<IUserBlockListRepository>();
                }

                return this.userBlockListRepository;
            }
        }

        public IUserLocationRepository UserLocationRepository
        {
            get
            {
                if (null == this.userLocationRepository)
                {
                    this.userLocationRepository = this.dependencyContainer.Resolve<IUserLocationRepository>();
                }

                return this.userLocationRepository;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
