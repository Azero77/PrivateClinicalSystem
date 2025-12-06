
using ClinicApp.Shared.QueryTypes;

namespace ClinicApp.Application.QueryServices;
public interface IQueryService<T>
    where T : class
{
    IQueryable<T> GetItems();
    Task<T?> GetItemById(Guid id);
    Task<T?> GetItemByIdWithJoins(Guid id) => GetItemById(id);
}
