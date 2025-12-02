using ClinicApp.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Infrastructure.Configurations;
internal class OutBoxMessageEntityConfiguration : IEntityTypeConfiguration<OutBoxMessage>
{
    public void Configure(EntityTypeBuilder<OutBoxMessage> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasKey(e => e.Id);
    }
}
