﻿using ClinicApp.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Middlewares;

public class EventualConsistencyMiddleware
{
    private readonly RequestDelegate _next;
    internal const string DomainEventsKey = "DomainEventsKey";
    public EventualConsistencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context,CancellationToken token,IPublisher publisher,DbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
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
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });
        await _next(context);
    }
}
