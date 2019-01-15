using RestServer.DataAccess.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.DocumentDb.Interfaces.Strategies
{
    public interface IUserLocationDataStoreStrategy : IDocumentDbDataStoreStrategy<UserLocation>
    {
        Task<UserLocation> GetUserLocation(int userId);

        Task<UserLocation> InsertOrUpdateUserLocation(UserLocation userLocation);
    }
}
