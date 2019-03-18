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
        Stopped = 3,
        Expired = 4 // This state will be set by a periodic process. Marking a capture session as expired is important as there would be numerous records in expired state
            // and while fetching the active records a date/time comparison for all such records would be required.
    }
}
