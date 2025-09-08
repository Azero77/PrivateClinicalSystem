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
}