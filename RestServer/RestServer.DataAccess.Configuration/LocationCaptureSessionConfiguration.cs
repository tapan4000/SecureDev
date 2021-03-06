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
    public class LocationCaptureSessionConfiguration : EntityTypeConfiguration<LocationCaptureSession>
    {
        public LocationCaptureSessionConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.LocationCaptureSessionId);

            // Properties
            this.ToTable("LocationCaptureSession");
            this.Property(t => t.LocationCaptureSessionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Title)
                .HasColumnName("Title")
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ExpiryDateTime)
                .HasColumnName("ExpiryDateTime")
                .IsRequired();

            this.Property(t => t.LocationProviderUserId)
                .HasColumnName("LocationProviderUserId")
                .IsRequired();

            this.Property(t => t.LocationCaptureSessionStateId)
                .HasColumnName("LocationCaptureSessionStateId")
                .IsRequired();

            this.Property(t => t.LocationCaptureTypeId)
                .HasColumnName("LocationCaptureTypeId")
                .IsRequired();

            this.Property(t => t.GroupId)
                .HasColumnName("GroupId")
                .IsRequired();

            this.Property(t => t.RequestDateTime)
                .HasColumnName("RequestDateTime")
                .IsRequired();

            this.Property(t => t.StoppedBy)
                .HasColumnName("StoppedBy")
                .HasMaxLength(50);

            this.Property(t => t.StopDateTime)
                .HasColumnName("StopDateTime");

            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(50);
            this.Property(t => t.CreationDateTime).HasColumnName("CreationDateTime");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy").HasMaxLength(50);
            this.Property(t => t.LastModificationDateTime).HasColumnName("LastModificationDateTime");

            // Properties not part of database model
            this.Ignore(t => t.ObjectState);
        }
    }
}
