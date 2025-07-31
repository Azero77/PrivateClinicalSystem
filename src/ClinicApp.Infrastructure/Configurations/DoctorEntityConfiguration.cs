﻿using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class DoctorEntityConfiguration :
    MemberEntityConfiguration<Doctor>
    
{
    public override void Configure(EntityTypeBuilder<Doctor> builder)
    {
        base.Configure(builder);
        builder.HasKey(e => e.Id);

        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.DoctorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.OwnsOne(e => e.WorkingTime,eb =>
        {
            eb.Property(eb => eb.WorkingDays)
            .HasColumnName(nameof(WorkingDays))
            .HasConversion<byte>()
            .IsRequired();

            eb.Property(eb => eb.WorkingHours)
            .HasColumnName(nameof(WorkingHours));

            eb.OwnsMany(eb => eb.TimesOff, tof =>
            {
                tof.ToTable("Doctor_TimesOff");
                tof.HasKey("DoctorId","StartDate","EndDate");
                tof.Property(eeb => eeb.reason)
                .IsRequired(false)
                .HasMaxLength(255);
                tof.WithOwner().HasForeignKey("DoctorId");
                tof.Property(eeb => eeb.StartDate)
                .IsRequired();
                tof.Property(eeb => eeb.EndDate)
                .IsRequired();
            });
        });
        builder.Property(e => e.Major)
            .IsRequired(false);
    }
}
