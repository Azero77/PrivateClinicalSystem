using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;

namespace ClinicApp.Infrastructure.Repositories;

internal interface IRepository<T>
    where T: AggregateRoot
{
    Task<T?> GetById(Guid id);
    Task Save(T entity,CancellationToken token = default);
}


internal class Repository<T, TData>
    : IRepository<T>
    where T : AggregateRoot
    where TData : class
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;

    public Repository(AppDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public Task<T?> GetById(Guid id)
    {
        return _context.Set<T>()
            .SingleOrDefaultAsync(t => t.Id == id);
    }


    public async Task Save(T entity,CancellationToken token = default)
    {
        var events = entity.PopDomainEvents();
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
    }
}