using RestServer.Cache.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public class CacheMetadataProvider : ICacheMetadataProvider
    {
        private static readonly ConcurrentDictionary<Type, CacheMetadata> TypeCacheMetadata = new ConcurrentDictionary<Type, CacheMetadata>();

        public CacheMetadata GetCacheMetadata(Type type)
        {
            if (TypeCacheMetadata.ContainsKey(type))
            {
                return TypeCacheMetadata[type];
            }

            var cacheMetadata = GetCacheMetadataForEntity(type);
            if(null != cacheMetadata)
            {
                TypeCacheMetadata.TryAdd(type, cacheMetadata);
            }

            return cacheMetadata;
        }

        private CacheMetadata GetCacheMetadataForEntity(Type type)
        {
            var storageAttribute = (StorageTypeAttribute)Attribute.GetCustomAttribute(type, typeof(StorageTypeAttribute));
            if(null == storageAttribute)
            {
                // Defaulting to no caching if the storage attribute is not found.
                return new CacheMetadata { CacheHint = CacheHint.None, CacheArea = CacheArea.Default };
            }

            return new CacheMetadata
            {
                CacheHint = storageAttribute.CacheHint,
                CacheArea = storageAttribute.CacheArea,
                TimeToLiveInSeconds = storageAttribute.TimeToLiveInSeconds
            };
        }
    }
}
