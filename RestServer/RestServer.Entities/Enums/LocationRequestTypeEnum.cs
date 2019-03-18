using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.Enums
{
    public enum LocationGenerationTypeEnum
    {
        // We cannot differentiate between a lookout mode capture session and a emergency mode capture session as the trigger made by mobile app only
        // cares about whether a periodic location update needs to be made. Whether it is catering to a lookout mode or emergency session depend on the
        // start, stop and expire time for the capture session.
        Adhoc = 1,
        CaptureSession = 2
    }
}
