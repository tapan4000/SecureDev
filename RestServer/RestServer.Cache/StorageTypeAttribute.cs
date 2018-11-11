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

        private readonly int timeToLiveInMinutes;
        public StorageTypeAttribute(CacheHint cacheHint, CacheArea cacheArea = CacheArea.Default, int timeToLiveInMinutes = 0)
        {
            this.cacheHint = cacheHint;
            this.cacheArea = cacheArea;
            this.timeToLiveInMinutes = timeToLiveInMinutes;
        }

        public CacheHint CacheHint
        {
            get { return this.cacheHint; }
        }

        public CacheArea CacheArea
        {
            get { return this.cacheArea; }
        }

        public int TimeToLiveInMinutes
        {
            get { return this.timeToLiveInMinutes; }
        }
    }
}
