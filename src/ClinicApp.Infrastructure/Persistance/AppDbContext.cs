using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistance;

public partial class AppDbContext : DbContext
{

    public DbSet<DoctorDataModel> Doctors { get; set; } = null!;
    public DbSet<SessionDataModel> Sessions { get; set; } = null!;
    public DbSet<RoomDataModel> Rooms { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("data");
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
