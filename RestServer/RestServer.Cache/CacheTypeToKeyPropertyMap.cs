using RestServer.Entities.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache
{
    public static class CacheTypeToKeyPropertyMap
    {
        private static readonly ConcurrentDictionary<string, IList<Func<object, string>>> TypeToKeyPropertyMap = new ConcurrentDictionary<string, IList<Func<object, string>>>();

        static CacheTypeToKeyPropertyMap()
        {
            TypeToKeyPropertyMap.TryAdd(typeof(User).FullName,
                new List<Func<object, string>> {
                                                    obj => CacheHelper.GetCacheKey<User>(CacheHelper.GetCacheCategoryWithIdentifier(CacheConstants.UserByMobileNumber, ((User)obj).CompleteMobileNumber), null),
                                                    obj => CacheHelper.GetCacheKey<User>(((User)obj).UserId.ToString(), null)
                                               });
        }

        public static IList<string> GetUserBasedCacheFinalKeys(int userId, string isdCode, string mobileNumber)
        {
            var userByMobileCacheCategoryWithIdentifier = CacheHelper.GetCacheCategoryWithIdentifier(CacheConstants.UserByMobileNumber, isdCode + mobileNumber);
            return new List<string>
            {
                CacheHelper.GetCacheKey<User>(userByMobileCacheCategoryWithIdentifier, null),
                CacheHelper.GetCacheKey<User>(userId.ToString(), null)
            };
        }

        public static IList<string> GetFinalKeyListForType<TEntity>(TEntity entity)
        {
            var entityTypeName = typeof(TEntity).FullName;
            
            if (TypeToKeyPropertyMap.ContainsKey(entityTypeName))
            {
                IList<string> keyList = new List<string>();
                foreach (var keyGeneratingFunc in TypeToKeyPropertyMap[entityTypeName])
                {
                    keyList.Add(keyGeneratingFunc.Invoke(entity));
                }

                return keyList;
            }

            return null;
        }
    }
}
