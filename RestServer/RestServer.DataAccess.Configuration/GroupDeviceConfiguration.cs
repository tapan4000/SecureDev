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
    public class GroupDeviceConfiguration : EntityTypeConfiguration<GroupDevice>
    {
        public GroupDeviceConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.GroupDeviceId);

            // Properties
            this.ToTable("GroupDevice");
            this.Property(t => t.GroupDeviceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            this.Property(t => t.DeviceId)
                .HasColumnName("DeviceId")
                .IsRequired();

            this.Property(t => t.IsAdministratorAllowedToTriggerEmergencySession)
                .HasColumnName("IsAdministratorAllowedToTriggerEmergencySession")
                .IsRequired();

            this.Property(t => t.IsAdministratorAllowedToExtendEmergencySession)
                .HasColumnName("IsAdministratorAllowedToExtendEmergencySession")
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
