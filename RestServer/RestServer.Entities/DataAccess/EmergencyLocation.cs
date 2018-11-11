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
    [StorageType(CacheHint.None, CacheArea.EmergencySession)]
    public partial class EmergencyLocation : IEntityBase
    {
        public int EmergencyLocationId { get; set; }

        public string LatitudeEncrypted { get; set; }

        public string LongitudeEncrypted { get; set; }

        public string SpeedEncrypted { get; set; }

        public int EmergencySessionId { get; set; }

        public int SameLocationReportCount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
