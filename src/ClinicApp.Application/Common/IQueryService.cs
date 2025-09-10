using ClinicApp.Application.QueryTypes;

namespace ClinicApp.Application.QueryServices;
public interface IQueryService<T>
    where T : QueryType
{
    IQueryable<T> GetItems();
    IQueryable<T> GetItemById(Guid id);
}
