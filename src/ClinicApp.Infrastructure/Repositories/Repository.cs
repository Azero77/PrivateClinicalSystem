using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class Repository<T, TData>
    : IRepository<T>
    where T : AggregateRoot
    where TData : DataModel
{
    protected readonly AppDbContext _context;
    private readonly IPublisher _publisher;
    protected readonly IConverter<T, TData> _converter;
    public Repository(AppDbContext context, IPublisher publisher, IConverter<T, TData> converter)
    {
        _context = context;
        _publisher = publisher;
        _converter = converter;
    }
    public async Task<T?> GetById(Guid id)
    {
        TData? datamodel = await _context.Set<TData>()
            .SingleOrDefaultAsync(t => t.Id == id);
        return datamodel is null ? null : _converter.MapToEntity(datamodel);
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