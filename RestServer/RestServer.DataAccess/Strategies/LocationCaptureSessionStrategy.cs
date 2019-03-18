using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;
using RestServer.Entities.Enums;

namespace RestServer.DataAccess.Strategies
{
    public class LocationCaptureSessionStrategy : DataStoreStrategyBase<LocationCaptureSession>, ILocationCaptureSessionStrategy
    {
        public LocationCaptureSessionStrategy(IDataContext dataContext) : base(dataContext)
        {
        }

        public Task<IList<LocationCaptureSession>> GetEmergencySessions(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsActiveCaptureSessionAvailableAccrossGroups(int userId)
        {
            return this.IsRecordPresentByFilter(captureSession => captureSession.LocationProviderUserId == userId 
                && captureSession.LocationCaptureSessionStateId == (int)LocationCaptureSessionStateEnum.Active 
                && captureSession.ExpiryDateTime >= DateTime.UtcNow);
        }

        public Task<bool> IsRecentlyInactivatedCaptureSessionAvailableAccrossGroups(int userId, int postInactivationLocationUpdateAllowedPeriodInSeconds)
        {
            var virtualCurrentDateAfterDeductingInactivationRelaxationPeriod = DateTime.UtcNow.AddSeconds(-1 * postInactivationLocationUpdateAllowedPeriodInSeconds);

            // In certain scenarios when the user has stopped the session or the session has expired, if there is any delay in posting the location update to the server
            // because of intermittent mobile connectivity, then the location update may not happen as no active capture session would be available. In such scenario,
            // provide a buffer period of at least one day for the records to be synchronized, post which any posting will be prevented.
            return this.IsRecordPresentByFilter(captureSession => captureSession.LocationProviderUserId == userId
                && captureSession.ExpiryDateTime >= virtualCurrentDateAfterDeductingInactivationRelaxationPeriod);
        }
    }
}
