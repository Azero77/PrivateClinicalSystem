using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ClinicApp.Infrastructure.Common;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;
    public UnitOfWork(AppDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken token = default,params AggregateRoot[] entities)
    {
        var events =
            entities.SelectMany(e => e.PopDomainEvents());
        var eventQueue = new Queue<IDomainEvent>(events);
        var transaction = await _context.Database.BeginTransactionAsync(token);
        try
        {
            while (eventQueue.TryDequeue(out IDomainEvent? domainEvent))
            {
                await _publisher.Publish(domainEvent, token);
            }
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
        }
        finally
        {
            await transaction.DisposeAsync();
        }
        return await _context.SaveChangesAsync(token);
    }
}