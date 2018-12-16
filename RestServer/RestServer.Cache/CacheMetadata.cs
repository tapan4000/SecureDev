using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class CacheMetadata
    {
        public CacheHint CacheHint { get; set; }

        public CacheArea CacheArea { get; set; }

        public int? TimeToLiveInSeconds { get; set; }
    }
}
