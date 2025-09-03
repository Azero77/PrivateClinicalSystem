using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<OutBoxMessage> OutBoxMessages { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("domain");
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
