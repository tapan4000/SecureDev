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
    public class NotificationMessageTemplateConfiguration : EntityTypeConfiguration<NotificationMessageTemplate>
    {
        public NotificationMessageTemplateConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.NotificationMessageTemplateId);

            // Properties
            this.ToTable("NotificationMessageTemplate");
            this.Property(t => t.NotificationMessageTemplateId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.NotificationModeId)
                .HasColumnName("NotificationModeId")
                .IsRequired();

            this.Property(t => t.NotificationMessageTypeId)
                .HasColumnName("NotificationMessageTypeId")
                .IsRequired();

            this.Property(t => t.LanguageId)
                .HasColumnName("LanguageId")
                .IsRequired();

            this.Property(t => t.Subject)
                .HasColumnName("Subject")
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Body)
                .HasColumnName("Body")
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
