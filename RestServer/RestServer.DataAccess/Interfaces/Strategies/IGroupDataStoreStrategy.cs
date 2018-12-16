﻿using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Interfaces.Strategies
{
    public interface IGroupDataStoreStrategy : IDataStoreStrategy<Group>
    {
        Task<int> GetGroupCountByUserId(int userId);
    }
}
