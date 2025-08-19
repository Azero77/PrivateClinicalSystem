using ClinicApp.Domain.Common;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;
public interface IPaginatedRepository<T>
    where T : Entity
{
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber, int itemsPerPage);
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber);
}

public class PaginatedRepostiory<T> : IPaginatedRepository<T>
    where T : Entity
{
    private readonly AppDbContext _context;
    protected int ItemsPerPage { get; set; } = 5;
    public PaginatedRepostiory(AppDbContext context)
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
        return GetAll(pageNumber,ItemsPerPage);
    }
}