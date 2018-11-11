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
    public class EmergencyLocationConfiguration : EntityTypeConfiguration<EmergencyLocation>
    {
        public EmergencyLocationConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.EmergencyLocationId);

            // Properties
            this.ToTable("EmergencyLocation");
            this.Property(t => t.EmergencyLocationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.LatitudeEncrypted)
                .HasColumnName("LatitudeEncrypted")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LongitudeEncrypted)
                .HasColumnName("LongitudeEncrypted")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SpeedEncrypted)
                .HasColumnName("SpeedEncrypted")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EmergencySessionId)
                .HasColumnName("EmergencySessionId")
                .IsRequired();

            this.Property(t => t.SameLocationReportCount)
                .HasColumnName("SameLocationReportCount")
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
