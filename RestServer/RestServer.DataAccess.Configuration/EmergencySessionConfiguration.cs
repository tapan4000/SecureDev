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
    public class EmergencySessionConfiguration : EntityTypeConfiguration<EmergencySession>
    {
        public EmergencySessionConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.EmergencySessionId);

            // Properties
            this.ToTable("EmergencySession");
            this.Property(t => t.EmergencySessionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ExpiryDateTime)
                .HasColumnName("ExpiryDateTime")
                .IsRequired();

            this.Property(t => t.FirstNotifiedAdminUserId)
                .HasColumnName("FirstNotifiedAdminUserId")
                .IsRequired();

            this.Property(t => t.FirstNotifiedDateTime)
                .HasColumnName("FirstNotifiedDateTime")
                .IsRequired();

            this.Property(t => t.EmergencyTargetUserId)
                .HasColumnName("EmergencyTargetUserId")
                .IsRequired();

            this.Property(t => t.IsEmergencyRequestInProgress)
                .HasColumnName("IsEmergencyRequestInProgress")
                .IsRequired();

            this.Property(t => t.RequestDateTime)
                .HasColumnName("RequestDateTime")
                .IsRequired();

            this.Property(t => t.StoppedBy)
                .HasColumnName("StoppedBy")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.StopDateTime)
                .HasColumnName("StopDateTime")
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
