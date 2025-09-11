using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class SessionEntityConfiguration : IEntityTypeConfiguration<SessionDataModel>
{
    public void Configure(EntityTypeBuilder<SessionDataModel> builder)
    {
        builder.ToTable("Sessions");
        //builder.HasQueryFilter(s => !s.IsDeleted);
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StartTime).HasColumnName("StartTime").IsRequired();
        builder.Property(s => s.EndTime).HasColumnName("Endtime").IsRequired();
        builder.Property(s => s.Content).HasColumnName("Content").IsRequired(false);

        builder.Property(s => s.SessionStatus)
            .HasConversion<byte>()
            .IsRequired();

        //Before adding history management (temporary
    }
}
