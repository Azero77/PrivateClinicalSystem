using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace ClinicApp.Infrastructure.Interceptors;
public class InsertOutBoxMessagesInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private JsonSerializerSettings jsonSerializer = new() { TypeNameHandling = TypeNameHandling.All};
    public InsertOutBoxMessagesInterceptor(IClock clock, IHttpContextAccessor httpContextAccessor)
    {
        this._clock = clock;
        _httpContextAccessor = httpContextAccessor;
    }

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
        {
            await AddEventsToMessageBoxes(context);
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task AddEventsToMessageBoxes(DbContext context)
    {
        var newDomainEvents = context
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .SelectMany(e => e.PopDomainEvents())
            .ToList();
        var outBoxMessages = 
            newDomainEvents
            .Select(e => new OutBoxMessage(
                Guid.NewGuid(),
                e.GetType().Name,
                JsonConvert.SerializeObject(e,jsonSerializer),
                _clock.UtcNow.UtcDateTime
                ))
            .ToList();
        if (_httpContextAccessor?.HttpContext is not null)
        {
            Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value)
                && value is Queue<IDomainEvent> exisistingDomainEvents ?
                exisistingDomainEvents : new();

            newDomainEvents.ForEach(domainEventsQueue.Enqueue);

            _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
        }
        await context.Set<OutBoxMessage>().AddRangeAsync(outBoxMessages);
    }
}
