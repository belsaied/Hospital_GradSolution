using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Data.Configurations
{
    public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
    {
        public void Configure(EntityTypeBuilder<EmergencyContact> builder)
        {
            builder.ToTable("EmergencyContarcts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(200)
                   .IsRequired(); 

            builder.Property(x => x.RelationShip)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.Phone)
                   .HasMaxLength(11)
                   .IsRequired();

            builder.Property(x => x.Email)
                   .HasMaxLength(100);

            builder.Property(e => e.Notes)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Patient)
                   .WithMany(P => P.EmergencyContacts)
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}
