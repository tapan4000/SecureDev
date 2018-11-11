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
    public partial class EmergencySessionExtension : IEntityBase
    {
        public int EmergencySessionExtensionId { get; set; }

        public int EmergencySessionId { get; set; }

        public DateTime? RequestDateTime { get; set; }

        public bool IsExtensionRequestInProgress { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}