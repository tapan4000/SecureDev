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
    [StorageType(CacheHint.DistributedCache, CacheArea.LocationSession, 43200)]
    public partial class LocationCaptureSession : IEntityBase
    {
        public int LocationCaptureSessionId { get; set; }

        public string Title { get; set; }

        public DateTime ExpiryDateTime { get; set; }

        public int LocationProviderUserId { get; set; }

        public int LocationCaptureSessionStateId { get; set; }

        public int LocationCaptureTypeId { get; set; }

        public int GroupId { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string StoppedBy { get; set; }

        public DateTime? StopDateTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }
        
        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
