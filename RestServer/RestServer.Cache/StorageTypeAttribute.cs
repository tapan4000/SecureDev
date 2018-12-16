using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class StorageTypeAttribute : Attribute
    {
        private readonly CacheHint cacheHint;

        private readonly CacheArea cacheArea;

        private readonly int timeToLiveInSeconds;

        public StorageTypeAttribute(CacheHint cacheHint, CacheArea cacheArea = CacheArea.Default, int timeToLiveInSeconds = 0)
        {
            this.cacheHint = cacheHint;
            this.cacheArea = cacheArea;
            this.timeToLiveInSeconds = timeToLiveInSeconds;
        }

        public CacheHint CacheHint
        {
            get { return this.cacheHint; }
        }

        public CacheArea CacheArea
        {
            get { return this.cacheArea; }
        }

        public int TimeToLiveInSeconds
        {
            get { return this.timeToLiveInSeconds; }
        }
    }
}
