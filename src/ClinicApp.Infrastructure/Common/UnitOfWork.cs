using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;

namespace ClinicApp.Infrastructure.Common;
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UnitOfWork(DbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        var newDomainEvents = _context.ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(domainEventList => domainEventList)
            .ToList();
        var result = await _context.SaveChangesAsync(token);
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
}
