using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicApp.Infrastructure.Configurations;

public class SessionEntityConfiguration : IEntityTypeConfiguration<SessionDataModel>
{
    public void Configure(EntityTypeBuilder<SessionDataModel> builder)
    {
        builder.ToTable("Sessions");
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

        builder.HasOne(e => e.Doctor)
            .WithMany(d => d.Sessions)
            .HasForeignKey(s => s.DoctorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Patient)
            .WithMany(p => p.Sessions)
            .HasForeignKey(s => s.PatientId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull); //when a patient is deleted the session does not have to be removed
        builder.HasOne(e => e.Room)
            .WithMany()
            .HasForeignKey(s => s.RoomId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //Before adding history management (tempo   rary)
        builder.Ignore(s => s.SessionHistory);
    }
}
