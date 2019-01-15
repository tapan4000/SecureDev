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

        public static string GetCacheCategoryWithIdentifier(string cacheCategory, string cacheIdentifier)
        {
            return string.Format(CachedKeyFormat, cacheCategory, cacheIdentifier);
        }

        public static string GetCacheKey<T>(string cacheKeySuffix, string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                return $"{typeof(T).FullName}|{cacheKeySuffix}";
            }

            return $"{entityName}|{cacheKeySuffix}";
        }
    }
}
