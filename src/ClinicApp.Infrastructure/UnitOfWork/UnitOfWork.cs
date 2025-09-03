using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http;

namespace ClinicApp.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private List<AggregateRoot> _trackedAggregates = new();
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnitOfWork(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var newDomainEvents = _trackedAggregates
        .Select(e => e.PopDomainEvents())
        .SelectMany(domainEventList => domainEventList)
        .ToList();
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (_httpContextAccessor?.HttpContext is not null)
        {
            Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
                && value is Queue<IDomainEvent> exisistingDomainEvents ?
                exisistingDomainEvents : new();

            newDomainEvents.ForEach(domainEventsQueue.Enqueue);
            _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
        }
        return result;
    }

    public void Track(AggregateRoot entity)
    {
        _trackedAggregates.Add(entity);
    }
}