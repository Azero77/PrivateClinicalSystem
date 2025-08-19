using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.DataQueryHelpers;

public class PaginatedRepository<T> : IPaginatedRepository<T>
    where T : Entity
{
    private readonly AppDbContext _context;
    protected int ItemsPerPage { get; set; } = 5;
    public PaginatedRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<T>> GetAll(int pageNumber, int itemsPerPage)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .Take(itemsPerPage)
            .Skip(itemsPerPage * pageNumber)
            .ToListAsync();
    }

    public Task<IReadOnlyCollection<T>> GetAll(int pageNumber)
    {
        return GetAll(pageNumber, ItemsPerPage);
    }

    public async Task<IReadOnlyCollection<T>> GetItems(DataQueryOptions<T> opts)
    {
        //when reached here we are sure that the dataqueryoptions are correct
        var query = _context.Set<T>().AsNoTracking();

        query = opts.Apply(query);

        return await query.ToListAsync();
    }

}
