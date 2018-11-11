using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Cache.Interfaces
{
    public interface ICacheMetadataProvider
    {
        CacheMetadata GetCacheMetadata(Type type);
    }
}
