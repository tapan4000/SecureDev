using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Configuration
{
    public class ApplicationConfiguration : EntityTypeConfiguration<Application>
    {
        public ApplicationConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.ApplicationId);

            // Properties
            this.ToTable("Application");
            this.Property(t => t.ApplicationId)
                .IsRequired();

            this.Property(t => t.ApplicationUniqueId)
                .HasColumnName("ApplicationUniqueId")
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
