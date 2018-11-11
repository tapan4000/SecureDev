using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public static class InMemoryCacheManager
    {
        /// <summary>
        /// The default cache expiration duration in seconds set to 1 day.
        /// </summary>
        private const int DefaultCacheExpirationDurationInSeconds = 86400;

        private static readonly MemoryCache MemCache = MemoryCache.Default;

        public static void Add<T>(string key, T value, int expirationDurationInSeconds = DefaultCacheExpirationDurationInSeconds, bool isSlidingExpiration = false)
        {
            try
            {
                AddInternal<T>(key, value, expirationDurationInSeconds, isSlidingExpiration);
            }
            catch (Exception ex)
            {
                throw new CacheException($"Error occurred while adding the key '{key}' to in-memory cache.", ex);
            }
        }

        public static void Add(string key, string value, int expirationDurationInSeconds = DefaultCacheExpirationDurationInSeconds, bool isSlidingExpiration = false)
        {
            try
            {
                AddInternal<string>(key, value, expirationDurationInSeconds, isSlidingExpiration);
            }
            catch (Exception ex)
            {
                throw new CacheException($"Error occurred while adding the key '{key}' to in-memory cache.", ex);
            }
        }

        public static T Get<T>(string key)
        {
            try
            {
                return GetInternal<T>(key);
            }
            catch(Exception ex)
            {
                throw new CacheException($"Error occurred while fetching the key '{key}'.", ex);
            }
        }

        public static string Get(string key)
        {
            try
            {
                return GetInternal<string>(key);
            }
            catch (Exception ex)
            {
                throw new CacheException($"Error occurred while fetching the key '{key}'.", ex);
            }
        }

        public static bool ContainsKey(string key)
        {
            try
            {
                ThrowArgumentExceptionIfNullKey(key);
                return MemCache.Contains(key);
            }
            catch(Exception ex)
            {
                throw new CacheException($"Error occurred while checking if key '{key}' exists.", ex);
            }
        }

        public static void Remove(string key)
        {
            try
            {
                MemCache.Remove(key);
            }
            catch (Exception ex)
            {
                throw new CacheException($"Error occurred while removing key '{key}'.", ex);
            }
        }

        public static void ClearCache()
        {
            try
            {
                Parallel.ForEach(MemCache, entry => MemCache.Remove(entry.Key));
            }
            catch(Exception ex)
            {
                throw new CacheException("Error occured while clearing the cache", ex);
            }
        }

        /// <summary>
        /// Clears the cache keys starting with a keyword. If we need to delete the cache keys from External storage, then use that as the prefix. Same goes for other configuration
        /// store as well.
        /// </summary>
        /// <param name="keyPrefix">The key prefix.</param>
        /// <exception cref="CacheException"></exception>
        public static void ClearCacheKeysStartingWith(string keyPrefix)
        {
            try
            {
                Parallel.ForEach(MemCache, entry =>
                {
                    if (entry.Key.StartsWith(keyPrefix))
                    {
                        MemCache.Remove(entry.Key);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new CacheException($"Error occured while clearing the cache with prefix: {keyPrefix}", ex);
            }
        }

        private static T GetInternal<T>(string key)
        {
            ThrowArgumentExceptionIfNullKey(key);

            var storedValue = MemCache.Get(key);
            if(null != storedValue)
            {
                return  (T)storedValue;
            }

            return default(T);
        }

        private static void AddInternal<T>(string key, T value, int expirationDurationInSeconds, bool isSlidingExpiration)
        {
            ThrowArgumentExceptionIfNullKey(key);

            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                throw new ArgumentException($"Received null value for key: {key}");
            }

            var cacheItemPolicy = new CacheItemPolicy { Priority = CacheItemPriority.Default };

            if (isSlidingExpiration)
            {
                cacheItemPolicy.SlidingExpiration = new TimeSpan(0, 0, expirationDurationInSeconds);
            }
            else
            {
                cacheItemPolicy.AbsoluteExpiration = (expirationDurationInSeconds > 0) ? DateTime.UtcNow.AddSeconds(expirationDurationInSeconds) : ObjectCache.InfiniteAbsoluteExpiration;
            }

            MemCache.Set(new CacheItem(key, value), cacheItemPolicy);
        }

        private static void ThrowArgumentExceptionIfNullKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be empty.");
            }
        }
    }
}
