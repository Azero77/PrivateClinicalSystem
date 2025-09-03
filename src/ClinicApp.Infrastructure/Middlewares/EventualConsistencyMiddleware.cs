using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicApp.Infrastructure.Middlewares;

public class EventualConsistencyMiddleware
{
    private readonly RequestDelegate _next;
    internal const string DomainEventsKey = "DomainEventsKey";
    private readonly ILogger<EventualConsistencyMiddleware> _logger;
    public EventualConsistencyMiddleware(RequestDelegate next, ILogger<EventualConsistencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context,IPublisher publisher,AppDbContext dbContext)
    {
        var token = context.RequestAborted;
        var transaction = await dbContext.Database.BeginTransactionAsync(token);
        context.Response.OnCompleted(async () =>
        {
            try
            {

                if (context.Items.TryGetValue(DomainEventsKey, out var result) && result is Queue<IDomainEvent> events)
                {
                    while (events.TryDequeue(out IDomainEvent? domainEvent))
                    {
                        await publisher.Publish(domainEvent, token);
                    }
                }

                await transaction.CommitAsync();
            }
            catch(Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error:" + e.ToString());
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });
        await _next(context);
    }
}