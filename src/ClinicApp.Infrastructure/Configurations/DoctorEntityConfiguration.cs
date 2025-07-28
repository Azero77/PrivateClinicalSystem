using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class DoctorEntityConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.DoctorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}
