namespace RestServer.Entities.DataAccess
{
    using Cache;
    using Cache.Core.Attributes;
    using Cache.Core.Enums;
    using Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Serializable]
    [StorageType(CacheHint.DistributedCache, CacheArea.Group)]
    public partial class LocalityBasedPublicGroup : IEntityBase
    {
        public int LocalityBasedPublicGroupId { get; set; }

        public int LocalityId { get; set; }

        public int PublicGroupId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
