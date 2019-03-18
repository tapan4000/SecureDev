using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Configuration
{
    public class MembershipTierConfiguration : EntityTypeConfiguration<MembershipTier>
    {
        public MembershipTierConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.MembershipTierId);

            // Properties
            this.ToTable("MembershipTier");

            this.Property(t => t.MembershipTierId)
                .HasColumnName("MembershipTierId")
                .IsRequired();

            this.Property(t => t.TierName)
                .HasColumnName("TierName")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EmergencySessionMaxDurationInSeconds)
                .HasColumnName("EmergencySessionMaxDurationInSeconds")
                .IsRequired();

            this.Property(t => t.EmergencySessionAvailabilityInSeconds)
                .HasColumnName("EmergencySessionAvailabilityInSeconds")
                .IsRequired();

            this.Property(t => t.LookoutSessionMaxDurationInSeconds)
                .HasColumnName("LookoutSessionMaxDurationInSeconds")
                .IsRequired();

            this.Property(t => t.LookoutSessionAvailabilityInSeconds)
                .HasColumnName("LookoutSessionAvailabilityInSeconds")
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
