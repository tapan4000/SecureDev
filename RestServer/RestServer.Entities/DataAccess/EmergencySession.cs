namespace RestServer.Entities.DataAccess
{
    using Cache;
    using Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Serializable]
    [StorageType(CacheHint.DistributedCache, CacheArea.EmergencySession)]
    public partial class EmergencySession : IEntityBase
    {
        public int EmergencySessionId { get; set; }

        public string Title { get; set; }

        public DateTime ExpiryDateTime { get; set; }

        public int? FirstNotifiedAdminUserId { get; set; }

        public DateTime? FirstNotifiedDateTime { get; set; }

        public int EmergencyTargetUserId { get; set; }

        public bool IsEmergencyRequestInProgress { get; set; }

        public DateTime? RequestDateTime { get; set; }

        public string StoppedBy { get; set; }

        public DateTime? StopDateTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }
        
        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
