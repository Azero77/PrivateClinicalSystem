
namespace ClinicApp.Application.QueryServices;
public interface IQueryService<T>
    where T : class
{
    IQueryable<T> GetItems();
    Task<T?> GetItemById(Guid id);
}
