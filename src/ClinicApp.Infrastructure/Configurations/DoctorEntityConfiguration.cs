using ClinicApp.Domain.Common.Entities;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class DoctorEntityConfiguration :
    MemberEntityConfiguration<DoctorDataModel>
    
{
    public override void Configure(EntityTypeBuilder<DoctorDataModel> builder)
    {
        base.Configure(builder);
        builder.ToTable("Doctors");
        builder.HasKey(e => e.Id);
        builder.HasOne(d => d.Room)
                .WithOne(r => r.Docotor)
                .HasForeignKey<DoctorDataModel>(d => d.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.Major)
            .HasMaxLength(255)
            .IsRequired(false);
        builder.HasMany(d => d.Sessions)
            .WithOne(s => s.Doctor)
            .HasForeignKey(s => s.DoctorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(d => d.WorkingDays)
            .HasColumnName("WorkingDays")
            .HasConversion<byte>();

        builder.Property(d => d.StartTime).HasColumnName("StartTime");
        builder.Property(d => d.EndTime).HasColumnName("EndTime");
        builder.Property(d => d.TimeZoneId);

        builder.OwnsMany<TimeOffDataModel>("TimesOff", tof =>
        {
            tof.ToTable("Doctor_TimesOff");
            tof.WithOwner().HasForeignKey("DoctorId");
            tof.HasKey("DoctorId", "StartDate", "EndDate");
            tof.Property(eeb => eeb.reason)
            .IsRequired(false)
            .HasMaxLength(255);
            tof.Property(eeb => eeb.StartDate)
            .IsRequired();
            tof.Property(eeb => eeb.EndDate)
            .IsRequired();
        });

        builder.Property(e => e.Major)
            .IsRequired(false);
    }
}
