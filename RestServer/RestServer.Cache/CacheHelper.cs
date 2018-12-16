using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class CacheHelper
    {
        private const string CachedKeyFormat = "{0}_{1}";

        public static string GetCacheKey(string cacheCategory, string cacheIdentifier)
        {
            return string.Format(CachedKeyFormat, cacheCategory, cacheIdentifier);
        }
    }
}
