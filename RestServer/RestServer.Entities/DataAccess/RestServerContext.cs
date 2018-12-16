namespace RestServer.Entities.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RestServerContext : DbContext
    {
        public RestServerContext()
            : base("name=RestServerContext")
        {
        }

        public virtual DbSet<CityBasedPublicGroup> CityBasedPublicGroups { get; set; }
        public virtual DbSet<CountryBasedPublicGroup> CountryBasedPublicGroups { get; set; }
        public virtual DbSet<EmergencyLocation> EmergencyLocations { get; set; }
        public virtual DbSet<EmergencySession> EmergencySessions { get; set; }
        public virtual DbSet<EmergencySessionExtension> EmergencySessionExtensions { get; set; }
        public virtual DbSet<EmergencySessionPublicGroupAccess> EmergencySessionPublicGroupAccesses { get; set; }
        public virtual DbSet<EmergencySessionPublicGroupAccess> EmergencySessionViewers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupCategory> GroupCategories { get; set; }
        public virtual DbSet<GroupDevice> GroupDevices { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<LocalityBasedPublicGroup> LocalityBasedPublicGroups { get; set; }
        public virtual DbSet<PublicGroup> PublicGroups { get; set; }
        public virtual DbSet<StateBasedPublicGroup> StateBasedPublicGroups { get; set; }
        public virtual DbSet<Device> SupplierDevices { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserActivation> UserActivations { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CityBasedPublicGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CityBasedPublicGroup>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CountryBasedPublicGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<CountryBasedPublicGroup>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyLocation>()
                .Property(e => e.LatitudeEncrypted)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyLocation>()
                .Property(e => e.LongitudeEncrypted)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyLocation>()
                .Property(e => e.SpeedEncrypted)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyLocation>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencyLocation>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySession>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySession>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySession>()
                .Property(e => e.StoppedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySession>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionExtension>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionExtension>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionPublicGroupAccess>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionPublicGroupAccess>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionPublicGroupAccess>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EmergencySessionPublicGroupAccess>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Group>()
                .Property(e => e.GroupName)
                .IsUnicode(false);

            modelBuilder.Entity<Group>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Group>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupCategory>()
                .Property(e => e.CategoryName)
                .IsUnicode(false);

            modelBuilder.Entity<GroupCategory>()
                .Property(e => e.CategoryDescription)
                .IsUnicode(false);

            modelBuilder.Entity<GroupCategory>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupCategory>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupDevice>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupDevice>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupMember>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<GroupMember>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<LocalityBasedPublicGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<LocalityBasedPublicGroup>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PublicGroup>()
                .Property(e => e.VerifiedTitle)
                .IsUnicode(false);

            modelBuilder.Entity<PublicGroup>()
                .Property(e => e.VerifiedDescription)
                .IsUnicode(false);

            modelBuilder.Entity<PublicGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PublicGroup>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<StateBasedPublicGroup>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<StateBasedPublicGroup>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.DeviceCode)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.DeviceRegistrationCodeEncrypted)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Device>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.UserUniqueId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.MobileNumber)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.PasswordHash)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserActivation>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserDevice>()
                .Property(e => e.DeviceFriendlyName)
                .IsUnicode(false);

            modelBuilder.Entity<UserDevice>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserDevice>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserSession>()
                .Property(e => e.RefreshToken)
                .IsUnicode(false);

            modelBuilder.Entity<UserSession>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserSession>()
                .Property(e => e.LastModifiedBy)
                .IsUnicode(false);
        }
    }
}
