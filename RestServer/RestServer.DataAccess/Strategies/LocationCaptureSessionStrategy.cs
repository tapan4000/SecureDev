using RestServer.DataAccess.Core.Strategies;
using RestServer.DataAccess.Interfaces.Strategies;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestServer.DataAccess.Core.Interfaces;

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
    }
}
