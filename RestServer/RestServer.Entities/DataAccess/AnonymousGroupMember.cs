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
    [StorageType(CacheHint.None, CacheArea.Application)]
    public class AnonymousGroupMember : IEntityBase
    {
        public int AnonymousGroupMemberId { get; set; }

        public int GroupId { get; set; }

        public string AnonymousUserIsdCode { get; set; }

        public string AnonymousUserMobileNumber { get; set; }

        public int GroupMemberStateId { get; set; }

        public DateTime RequestExpiryDateTime { get; set; }

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
