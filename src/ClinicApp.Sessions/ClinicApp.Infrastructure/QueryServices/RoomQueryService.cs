using ClinicApp.Application.QueryServices;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Shared.QueryTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Infrastructure.QueryServices;

public class RoomQueryService(AppDbContext context) : IQueryService<RoomQueryType>
{
    private static Expression<Func<RoomDataModel, RoomQueryType>> _converter = r => new()
    {
        Id = r.Id,
        Name = r.Name
    };

    public async Task<RoomQueryType?> GetItemById(Guid id)
    {
        return await context.Rooms.Where(r => r.Id == id)
            .Select(_converter)
            .SingleOrDefaultAsync();
    }

    public IQueryable<RoomQueryType> GetItems()
    {
        return context.Rooms.Select(_converter);
    }
}
