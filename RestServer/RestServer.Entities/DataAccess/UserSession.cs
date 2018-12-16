namespace RestServer.Entities.DataAccess
{
    using Cache;
    using Core;
    using System;

    [Serializable]
    [StorageType(CacheHint.None, CacheArea.User)]
    public partial class UserSession : IEntityBase
    {
        public int UserId { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenCreationDateTime { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }
    }
}
