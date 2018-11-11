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
    public class StateBasedPublicGroupConfiguration : EntityTypeConfiguration<StateBasedPublicGroup>
    {
        public StateBasedPublicGroupConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.StateBasedPublicGroupId);

            // Properties
            this.ToTable("StateBasedPublicGroup");
            this.Property(t => t.StateBasedPublicGroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.StateId)
                .HasColumnName("StateId")
                .IsRequired();

            this.Property(t => t.PublicGroupId)
                .HasColumnName("PublicGroupId")
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
