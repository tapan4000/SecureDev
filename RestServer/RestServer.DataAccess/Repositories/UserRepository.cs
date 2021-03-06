﻿using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Cache.Interfaces;
using RestServer.Cache;
using RestServer.Logging.Interfaces;
using RestServer.Entities;

namespace RestServer.DataAccess.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly IUserDataStoreStrategy userDataStoreStrategy;

        private readonly ICacheStrategyHandler<User> cacheStrategyHandler;

        public UserRepository(IDependencyContainer dependencyContainer, IUserDataStoreStrategy dataStoreStrategy, ICacheStrategyHandler<User> cacheStrategyHandler, IEventLogger logger) 
            : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.userDataStoreStrategy = dataStoreStrategy;
            this.cacheStrategyHandler = cacheStrategyHandler;
        }

        public async Task<User> GetUserByMobileNumber(string isdCode, string mobileNumber)
        {
            var cacheKeyCategoryAndIdentifier = CacheHelper.GetCacheCategoryWithIdentifier(CacheConstants.UserByMobileNumber, isdCode + mobileNumber);
            var cachedUser = await this.cacheStrategyHandler.GetFromStoreAsync(cacheKeyCategoryAndIdentifier);
            if(null != cachedUser)
            {
                return cachedUser;
            }

            var user = await this.userDataStoreStrategy.GetUserByMobileNumber(isdCode, mobileNumber).ConfigureAwait(false);
            if(null != user)
            {
                await this.cacheStrategyHandler.InsertOrUpdateInStoreAsync(cacheKeyCategoryAndIdentifier, user);
            }

            return user;
        }
    }
}
