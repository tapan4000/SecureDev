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
    public class GroupCategoryConfiguration : EntityTypeConfiguration<GroupCategory>
    {
        public GroupCategoryConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.GroupCategoryId);

            // Properties
            this.ToTable("GroupCategory");
            this.Property(t => t.GroupCategoryId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CategoryName)
                .HasColumnName("CategoryName")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CategoryDescription)
                .HasColumnName("CategoryDescription")
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
            this.Property(t => t.CreationDateTime).HasColumnName("CreationDateTime");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(50);
            this.Property(t => t.LastModificationDateTime).HasColumnName("LastModificationDateTime");

            // Properties not part of database model
            this.Ignore(t => t.ObjectState);
        }
    }
}
