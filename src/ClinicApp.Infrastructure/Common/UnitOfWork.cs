using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var events = entities.SelectMany(e => e.PopDomainEvents()).ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync(token);

        try
        {
            foreach (var domainEvent in events)
            {
                await _publisher.Publish(domainEvent, token);
            }

            var result = await _context.SaveChangesAsync(token);

            await transaction.CommitAsync(token);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
        }
        return 0;
    }
}