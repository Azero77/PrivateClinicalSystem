using ClinicApp.Application.Common;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public DbSet<DoctorDataModel> Doctors { get; set; } = null!;
    public DbSet<SessionDataModel> Sessions { get; set; } = null!;
    public DbSet<PatientDataModel> Patients { get; set; } = null!;
    public DbSet<SecretaryDataModel> Secretaries { get; set; } = null!;
    public DbSet<RoomDataModel> Rooms { get; set; } = null!;
    public DbSet<SessionState> SessionStates { get; set; } = null!;
    public DbSet<OutBoxMessage> OutBoxMessages { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    static AppDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("domain");
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
