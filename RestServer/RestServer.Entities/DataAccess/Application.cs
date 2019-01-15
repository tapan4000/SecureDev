using RestServer.Cache;
using RestServer.Cache.Core.Attributes;
using RestServer.Cache.Core.Enums;
using RestServer.Entities.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Entities.DataAccess
{
    [Serializable]
    [StorageType(CacheHint.LocalCache, CacheArea.Application)]
    public class Application : IEntityBase
    {
        public int ApplicationId { get; set; }

        public string ApplicationUniqueId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
