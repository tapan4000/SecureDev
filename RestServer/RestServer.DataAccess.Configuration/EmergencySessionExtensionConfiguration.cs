﻿using RestServer.Entities.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.Configuration
{
    public class EmergencySessionExtensionConfiguration : EntityTypeConfiguration<EmergencySessionExtension>
    {
        public EmergencySessionExtensionConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.EmergencySessionExtensionId);

            // Properties
            this.ToTable("EmergencySessionExtension");
            this.Property(t => t.EmergencySessionExtensionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.EmergencySessionId)
                .HasColumnName("EmergencySessionId")
                .IsRequired();

            this.Property(t => t.RequestDateTime)
                .HasColumnName("RequestDateTime")
                .IsRequired();

            this.Property(t => t.IsExtensionRequestInProgress)
                .HasColumnName("IsExtensionRequestInProgress")
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