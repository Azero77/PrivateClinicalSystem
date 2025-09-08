using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class RoomEntityConfiguration : IEntityTypeConfiguration<RoomDataModel>
{
    public void Configure(EntityTypeBuilder<RoomDataModel> builder)
    {
        builder.ToTable("Rooms");
        builder.HasMany<Session>()
            .WithOne()
            .HasForeignKey(s => s.RoomId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
