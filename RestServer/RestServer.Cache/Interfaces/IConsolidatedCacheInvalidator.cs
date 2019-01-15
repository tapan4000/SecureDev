using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface IConsolidatedCacheInvalidator
    {
        void Register(ICacheStrategy cacheStrategy, string cacheKey);

        void Register(ICacheStrategy cacheStrategy, IList<string> cacheKeys);

        Task invalidateAsync();
    }
}
