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
    [StorageType(CacheHint.DistributedCache, CacheArea.User)]
    public partial class User : IEntityBase
    {
        public int UserId { get; set; }

        public string UserUniqueId { get; set; }

        public string IsdCode { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PasswordHash { get; set; }

        public UserState UserStateId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModificationDateTime { get; set; }

        public ObjectState ObjectState { get; set; }

        public string CompleteMobileNumber
        {
            get
            {
                return this.IsdCode + this.MobileNumber;
            }
        }
    }
}
