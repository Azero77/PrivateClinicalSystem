using ClinicApp.Application.Common;
using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;

namespace ClinicApp.Infrastructure.Common;
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync();
    }
}
