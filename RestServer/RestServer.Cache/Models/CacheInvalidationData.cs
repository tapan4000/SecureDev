using RestServer.Cache.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Models
{
    public class CacheInvalidationData
    {
        public CacheInvalidationData(ICacheStrategy cacheStrategy, string mergedKey)
        {
            this.CacheStrategy = cacheStrategy;
            this.CacheKey = mergedKey;
        }

        public ICacheStrategy CacheStrategy
        {
            get; private set;
        }

        public string CacheKey
        {
            get; private set;
        }
    }
}
