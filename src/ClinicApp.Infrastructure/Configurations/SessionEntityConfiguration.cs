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
