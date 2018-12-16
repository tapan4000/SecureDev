using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Configuration
{
    public class RestServerSettingConfiguration : EntityTypeConfiguration<RestServerSetting>
    {
        public RestServerSettingConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.Key);

            // Properties
            this.ToTable("RestServerSetting");

            this.Property(t => t.Key)
                .HasColumnName("Key")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Value)
                .HasColumnName("Value")
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