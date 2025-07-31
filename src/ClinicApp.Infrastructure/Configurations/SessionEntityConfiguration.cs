using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicApp.Infrastructure.Configurations;

public class SessionEntityConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);

        builder.OwnsOne(s => s.SessionDate,
            sd =>
            {
                sd.Property(sd => sd.StartTime).HasColumnName("StartTime").IsRequired();
                sd.Property(sd => sd.EndTime).HasColumnName("Endtime").IsRequired();
            });

        builder.Property(s => s.SessionStatus)
            .HasConversion<byte>()
            .IsRequired();

        builder.OwnsOne(s => s.SessionDescription,nb =>
        {
            nb.Property(nb => nb.content).HasColumnName("Content").IsRequired(false);
        });

        //Before adding history management (temporary)
        builder.Ignore(s => s.SessionHistory);
    }
}
