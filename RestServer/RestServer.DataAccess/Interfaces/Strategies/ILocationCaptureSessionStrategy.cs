using RestServer.DataAccess.Core.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Interfaces.Strategies
{
    public interface ILocationCaptureSessionStrategy : IDataStoreStrategy<LocationCaptureSession>
    {
        Task<IList<LocationCaptureSession>> GetEmergencySessions(int groupId);
    }
}
