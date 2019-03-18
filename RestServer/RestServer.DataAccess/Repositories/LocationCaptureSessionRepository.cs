using RestServer.DataAccess.Core.Interfaces.Repositories;
using RestServer.DataAccess.Core.Repositories;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.Cache.Interfaces;
using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.IoC.Interfaces;
using RestServer.Logging.Interfaces;
using RestServer.DataAccess.Interfaces.Strategies;

namespace RestServer.DataAccess.Repositories
{
    public class LocationCaptureSessionRepository : RepositoryBase<LocationCaptureSession>, ILocationCaptureSessionRepository
    {
        private ILocationCaptureSessionStrategy locationCaptureSessionStrategy;

        public LocationCaptureSessionRepository(IDependencyContainer dependencyContainer, ILocationCaptureSessionStrategy dataStoreStrategy, ICacheStrategyHandler<LocationCaptureSession> cacheStrategyHandler, IEventLogger logger) : base(dependencyContainer, dataStoreStrategy, cacheStrategyHandler, logger)
        {
            this.locationCaptureSessionStrategy = dataStoreStrategy;
        }

        public Task<IList<LocationCaptureSession>> GetEmergencySessions(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsActiveCaptureSessionAvailableAccrossGroups(int userId)
        {
            return await this.locationCaptureSessionStrategy.IsActiveCaptureSessionAvailableAccrossGroups(userId).ConfigureAwait(false);
        }

        public async Task<bool> IsRecentlyInactivatedCaptureSessionAvailableAccrossGroups(int userId, int postInactivationLocationUpdateAllowedPeriodInSeconds)
        {
            return await this.locationCaptureSessionStrategy.IsRecentlyInactivatedCaptureSessionAvailableAccrossGroups(userId, postInactivationLocationUpdateAllowedPeriodInSeconds).ConfigureAwait(false);
        }
    }
}
