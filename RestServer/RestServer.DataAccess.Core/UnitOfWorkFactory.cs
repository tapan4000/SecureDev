using RestServer.DataAccess.Core.Interfaces;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IDependencyContainer dependencyContainer;

        public UnitOfWorkFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IRestServerUnitOfWork RestServerUnitOfWork
        {
            get
            {
                return this.dependencyContainer.Resolve<IRestServerUnitOfWork>();
            }
        }

        public IDocumentDbUnitOfWork DocumentDbUnitOfWork
        {
            get
            {
                return this.dependencyContainer.Resolve<IDocumentDbUnitOfWork>();
            }
        }
    }
}
