//using System;
using ClinicApp.Domain.Patient;
using ClinicApp.Domain.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class PatientEntityConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.PatientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
