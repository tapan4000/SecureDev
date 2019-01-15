namespace RestServer.Entities.DataAccess
{
    using Cache;
    using Cache.Core.Attributes;
    using Cache.Core.Enums;
    using Core;
    using System;

    [Serializable]
    [StorageType(CacheHint.DistributedCache, CacheArea.User)]
    public partial class UserDevice : IEntityBase
    {
        public int UserDeviceId { get; set; }

        public int UserId { get; set; }

        public int DeviceId { get; set; }

        public string DeviceFriendlyName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
