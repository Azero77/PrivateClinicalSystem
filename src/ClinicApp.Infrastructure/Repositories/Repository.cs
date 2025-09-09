using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClinicApp.Infrastructure.Repositories;

public class Repository<T, TData>
    : IRepository<T>
    where T : AggregateRoot
    where TData : DataModel
{
    protected readonly AppDbContext _context;
    protected readonly IConverter<T, TData> _converter;
    public Repository(AppDbContext context, IConverter<T, TData> converter)
    {
        _context = context;
        _converter = converter;
    }
    public async Task<T?> GetById(Guid id)
    {
        TData? datamodel = await _context.Set<TData>()
            .SingleOrDefaultAsync(t => t.Id == id);
        return datamodel is null ? null : _converter.MapToEntity(datamodel);
    }

    /// <summary>
    /// Method For Updated Domain models that map it again to EF model and then run the update method
    /// wherease for create and delete , the repo in the specific domain will handle this logic
    /// </summary>
    /// <param name="Entity"></param>
    /// <returns></returns>
    public Task SaveAsync(T entity)
    {
        var dataModel = _converter.MapToData(entity);
        _context.Set<TData>().Update(dataModel);
        return Task.CompletedTask;
    }
}