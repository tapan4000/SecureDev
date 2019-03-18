using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.DocumentDb.Interfaces.Strategies;

namespace RestServer.DataAccess.DocumentDb.Repositories
{
    public class UserLocationRepository : DocumentDbRepositoryBase<UserLocation>, IUserLocationRepository
    {
        private IUserLocationDataStoreStrategy userLocationDataStoreStrategy;

        public UserLocationRepository(IUserLocationDataStoreStrategy documentDbDataStoreStrategy) : base(documentDbDataStoreStrategy)
        {
            this.userLocationDataStoreStrategy = documentDbDataStoreStrategy;
        }

        public Task<UserLocation> GetUserLocation(int userId)
        {
            return this.userLocationDataStoreStrategy.GetUserLocation(userId);
        }

        public Task<UserLocation> InsertOrUpdateUserLocation(UserLocation userLocation)
        {
            return this.userLocationDataStoreStrategy.InsertOrUpdateUserLocation(userLocation);
        }
    }
}
