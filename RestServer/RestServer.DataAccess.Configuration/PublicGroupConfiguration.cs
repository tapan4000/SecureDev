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
    public class PublicGroupConfiguration : EntityTypeConfiguration<PublicGroup>
    {
        public PublicGroupConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.GroupId);

            // Properties
            this.ToTable("PublicGroup");

            this.Property(t => t.GroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IsVerified)
                .HasColumnName("IsVerified")
                .IsRequired();

            this.Property(t => t.VerifiedTitle)
                .HasColumnName("VerifiedTitle")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.VerifiedDescription)
                .HasColumnName("VerifiedDescription")
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
