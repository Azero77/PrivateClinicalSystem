//using System;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class PatientEntityConfiguration : MemberEntityConfiguration<PatientDataModel>
{
    public override void Configure(EntityTypeBuilder<PatientDataModel> builder)
    {
        base.Configure(builder);
        builder.ToTable("Patients");
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.PatientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
