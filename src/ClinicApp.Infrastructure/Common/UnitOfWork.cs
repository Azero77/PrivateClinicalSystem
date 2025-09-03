using ClinicApp.Application.Common;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ClinicApp.Infrastructure.Common;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync(token);
    }
}