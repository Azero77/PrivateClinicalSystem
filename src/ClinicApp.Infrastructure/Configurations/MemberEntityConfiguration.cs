using ClinicApp.Domain.Common.Entities;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class MemberEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MemberDataModel
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(m => m.FirstName)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(m => m.LastName)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(m => m.UserId)
            .IsUnique();
    }
}