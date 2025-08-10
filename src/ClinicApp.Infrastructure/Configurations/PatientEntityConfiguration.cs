//using System;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class PatientEntityConfiguration : MemberEntityConfiguration<Patient>
{
    public override void Configure(EntityTypeBuilder<Patient> builder)
    {
        base.Configure(builder);
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.PatientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
