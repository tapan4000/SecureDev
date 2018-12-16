using RestServer.Cache;
using RestServer.Entities.DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities
{
    public static class CacheTypeToKeyPropertyMap
    {
        private static readonly ConcurrentDictionary<string, Func<object, string>> TypeToKeyPropertyMap = new ConcurrentDictionary<string, Func<object, string>>();

        static CacheTypeToKeyPropertyMap()
        {
            TypeToKeyPropertyMap.TryAdd(typeof(User).FullName, obj => ((User)obj).UserId.ToString());
            TypeToKeyPropertyMap.TryAdd(typeof(User).FullName, obj => CacheHelper.GetCacheKey(CacheConstants.UserByMobileNumber, ((User)obj).CompleteMobileNumber));
        }

        public static string GetKeyForType<TEntity>(TEntity entity)
        {
            if (TypeToKeyPropertyMap.ContainsKey(typeof(TEntity).FullName))
            {
                return TypeToKeyPropertyMap[typeof(TEntity).FullName].Invoke(entity);
            }

            return null;
        }
    }
}
