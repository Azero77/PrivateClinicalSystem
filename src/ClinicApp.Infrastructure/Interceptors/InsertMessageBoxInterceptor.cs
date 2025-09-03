using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace ClinicApp.Infrastructure.Interceptors;
public class InsertMessageBoxInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;

    public InsertMessageBoxInterceptor(IClock clock)
    {
        this._clock = clock;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData,
                                                     int result,
                                                     CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is not null)
        {
            await AddEventsToMessageBoxes(context);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task AddEventsToMessageBoxes(DbContext context)
    {
        var outBoxMessages = context
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .SelectMany(e => e.PopDomainEvents())
            .Select(e => new OutBoxMessage(
                Guid.NewGuid(),
                e.GetType().Name,
                JsonSerializer.Serialize(e),
                _clock.UtcNow.UtcDateTime
                ))
            .ToList();

        await context.Set<OutBoxMessage>().AddRangeAsync(outBoxMessages);
    }
}
