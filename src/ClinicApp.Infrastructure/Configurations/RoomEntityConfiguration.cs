using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.SessionAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class RoomEntityConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.RoomId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
