using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var newDomainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(domainEventList => domainEventList)
            .ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
            && value is Queue<IDomainEvent> exisistingDomainEvents ?
            exisistingDomainEvents : new();

        newDomainEvents.ForEach(domainEventsQueue.Enqueue);

        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = newDomainEvents;
        return result;
    }
}
