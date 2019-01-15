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
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            // Primary key
            this.HasKey(t => t.UserId);

            // Properties
            this.ToTable("User");
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.UserUniqueId)
                .HasColumnName("UserUniqueId")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IsdCode)
                .HasColumnName("IsdCode")
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.MobileNumber)
                .HasColumnName("MobileNumber")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PasswordHash)
                .IsRequired()
                .HasColumnName("PasswordHash")
                .HasMaxLength(50);

            this.Property(t => t.UserStateId)
                .IsRequired()
                .HasColumnName("UserStateId");

            this.Property(t => t.IsGroupMemberRequestSynchronized)
                .IsRequired()
                .HasColumnName("IsGroupMemberRequestSynchronized");

            this.Property(t => t.MembershipTierId)
                .IsRequired()
                .HasColumnName("MembershipTierId");

            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
            this.Property(t => t.CreationDateTime).HasColumnName("CreationDateTime");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(50);
            this.Property(t => t.LastModificationDateTime).HasColumnName("LastModificationDateTime");

            // Properties not part of database model
            this.Ignore(t => t.CompleteMobileNumber);
            this.Ignore(t => t.ObjectState);
        }
    }
}
