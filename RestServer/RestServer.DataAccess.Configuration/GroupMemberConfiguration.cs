using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Configuration
{
    public class GroupMemberConfiguration : EntityTypeConfiguration<GroupMember>
    {
        public GroupMemberConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.GroupMemberId);

            // Properties
            this.ToTable("GroupMember");
            this.Property(t => t.GroupMemberId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            this.Property(t => t.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            this.Property(t => t.IsAdministratorAllowedToTriggerEmergencySession)
                .HasColumnName("IsAdministratorAllowedToTriggerEmergencySession")
                .IsRequired();

            this.Property(t => t.IsAdministratorAllowedToExtendEmergencySession)
                .HasColumnName("IsAdministratorAllowedToExtendEmergencySession")
                .IsRequired();

            this.Property(t => t.EmergencyNotificationModePreference)
                .HasColumnName("EmergencyNotificationModePreference")
                .IsRequired();

            this.Property(t => t.IsAdmin)
                .HasColumnName("IsAdmin")
                .IsRequired();

            this.Property(t => t.IsPrimary)
                .HasColumnName("IsPrimary")
                .IsRequired();

            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
            this.Property(t => t.CreationDateTime).HasColumnName("CreationDateTime");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(50);
            this.Property(t => t.LastModificationDateTime).HasColumnName("LastModificationDateTime");

            // Properties not part of database model
            this.Ignore(t => t.ObjectState);
        }
    }
}
