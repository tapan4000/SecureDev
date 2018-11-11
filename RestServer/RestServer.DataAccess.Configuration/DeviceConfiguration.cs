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
    public class DeviceConfiguration : EntityTypeConfiguration<Device>
    {
        public DeviceConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.DeviceId);

            // Properties
            this.ToTable("Device");
            this.Property(t => t.DeviceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SupplierId)
                .HasColumnName("SupplierId")
                .IsRequired();

            this.Property(t => t.DeviceCode)
                .HasColumnName("DeviceCode")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DeviceType)
                .HasColumnName("DeviceType")
                .IsRequired();

            this.Property(t => t.DeviceRegistrationCodeEncrypted)
                .HasColumnName("DeviceRegistrationCodeEncrypted")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
            this.Property(t => t.CreationDateTime).HasColumnName("CreationDateTime");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(50);
            this.Property(t => t.LastModificationDateTime).HasColumnName("LastModificationDateTime");

            // Properties not part of database model
            this.Ignore(t => t.ObjectState);
        }
    }
}
