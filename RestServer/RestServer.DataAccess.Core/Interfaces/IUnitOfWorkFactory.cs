﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IRestServerUnitOfWork RestServerUnitOfWork { get; }

        IDocumentDbUnitOfWork DocumentDbUnitOfWork { get; }
    }
}
