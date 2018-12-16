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
    public class UserSessionConfiguration : EntityTypeConfiguration<UserSession>
    {
        public UserSessionConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.ToTable("UserSession");

            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RefreshToken)
                .HasColumnName("RefreshToken")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RefreshTokenCreationDateTime)
                .HasColumnName("RefreshTokenCreationDateTime")
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
