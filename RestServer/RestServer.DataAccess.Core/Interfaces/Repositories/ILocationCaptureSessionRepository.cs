using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Core.Interfaces.Repositories
{
    public interface ILocationCaptureSessionRepository : IRepository<LocationCaptureSession>
    {
        Task<IList<LocationCaptureSession>> GetEmergencySessions(int groupId);

        Task<bool> IsActiveCaptureSessionAvailableAccrossGroups(int userId);

        Task<bool> IsRecentlyInactivatedCaptureSessionAvailableAccrossGroups(int userId, int postInactivationLocationUpdateAllowedPeriodInSeconds);
    }
}
