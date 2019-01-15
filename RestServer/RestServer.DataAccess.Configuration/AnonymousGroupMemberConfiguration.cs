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
    public class AnonymousGroupMemberConfiguration : EntityTypeConfiguration<AnonymousGroupMember>
    {
        public AnonymousGroupMemberConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.AnonymousGroupMemberId);

            // Properties
            this.ToTable("AnonymousGroupMember");
            this.Property(t => t.AnonymousGroupMemberId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.GroupId)
                .IsRequired()
                .HasColumnName("GroupId");

            this.Property(t => t.AnonymousUserIsdCode)
                .HasColumnName("AnonymousUserIsdCode")
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.AnonymousUserMobileNumber)
                .HasColumnName("AnonymousUserMobileNumber")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.GroupMemberStateId)
                .IsRequired()
                .HasColumnName("GroupMemberStateId");

            this.Property(t => t.RequestExpiryDateTime)
                .IsRequired()
                .HasColumnName("RequestExpiryDateTime");

            this.Property(t => t.CanAdminTriggerEmergencySessionForSelf)
                .HasColumnName("CanAdminTriggerEmergencySessionForSelf")
                .IsRequired();

            this.Property(t => t.CanAdminExtendEmergencySessionForSelf)
                .HasColumnName("CanAdminExtendEmergencySessionForSelf")
                .IsRequired();

            this.Property(t => t.GroupPeerEmergencyNotificationModePreferenceId)
                .HasColumnName("GroupPeerEmergencyNotificationModePreferenceId")
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
