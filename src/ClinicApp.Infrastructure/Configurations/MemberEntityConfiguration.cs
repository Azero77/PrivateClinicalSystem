using ClinicApp.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicApp.Infrastructure.Configurations;

public class MemberEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : Member
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(m => m.FirstName)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(m => m.LastName)
            .HasMaxLength(255)
            .IsRequired();
    }
}