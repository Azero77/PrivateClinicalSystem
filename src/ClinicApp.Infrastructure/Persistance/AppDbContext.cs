using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public DbSet<DoctorDataModel> Doctors { get; set; } = null!;
    public DbSet<SessionDataModel> Sessions { get; set; } = null!;
    public DbSet<RoomDataModel> Rooms { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("data");
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /*public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var newDomainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(domainEventList => domainEventList)
            .ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        if (_httpContextAccessor?.HttpContext is not null)
        {
            Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
                && value is Queue<IDomainEvent> exisistingDomainEvents ?
                exisistingDomainEvents : new();

            newDomainEvents.ForEach(domainEventsQueue.Enqueue);

            _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
        }
        return result;
    }*/
}
