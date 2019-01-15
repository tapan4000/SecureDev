using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Enums
{
    public enum LocationCaptureSessionStateEnum
    {
        None = 0,
        PendingSyncWithLocationProvider = 1,
        Active = 2,
    }
}
