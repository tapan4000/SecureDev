using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public enum CacheArea
    {
        Default = 0,
        User = 1,
        Group = 2,
        EmergencySession = 3
    }
}
