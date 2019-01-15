using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Core.Enums
{
    public enum CacheHint
    {
        None = 0,
        DistributedCache = 1,
        LocalCache = 2
    }
}
