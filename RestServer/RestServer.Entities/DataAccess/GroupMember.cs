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
    [StorageType(CacheHint.DistributedCache, CacheArea.Group)]
    public partial class GroupMember : IEntityBase
    {
        public int GroupMemberId { get; set; }

        public int GroupId { get; set; }

        public int UserId { get; set; }

        public bool CanAdminTriggerEmergencySessionForSelf { get; set; }

        public bool CanAdminExtendEmergencySessionForSelf { get; set; }

        public int GroupPeerEmergencyNotificationModePreferenceId { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsPrimary { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
