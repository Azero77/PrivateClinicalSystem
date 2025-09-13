using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Infrastructure.QueryServices;

public class SecretaryQueryService(AppDbContext context) : IQueryService<SecretaryQueryType>
{
    private static Expression<Func<SecretaryDataModel, SecretaryQueryType>> _converter = s => new()
    {
        Id = s.Id,
        FirstName = s.FirstName,
        LastName = s.LastName,
        UserId = s.UserId
    };

    public async Task<SecretaryQueryType?> GetItemById(Guid id)
    {
        return await context.Secretaries.Where(s => s.Id == id)
            .Select(_converter)
            .SingleOrDefaultAsync();
    }

    public IQueryable<SecretaryQueryType> GetItems()
    {
        return context.Secretaries.Select(_converter);
    }
}
