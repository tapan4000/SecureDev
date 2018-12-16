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
    [StorageType(CacheHint.None, CacheArea.User)]
    public partial class UserActivation : IEntityBase
    {
        public int UserId { get; set; }

        public int ActivationCode { get; set; }

        public int TotalOtpGenerationAttemptCount { get; set; }

        public int CurrentWindowOtpGenerationAttemptCount { get; set; }

        public DateTime NextOtpGenerationWindowStartDateTime { get; set; }

        public DateTime UserActivationExpiryDateTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
