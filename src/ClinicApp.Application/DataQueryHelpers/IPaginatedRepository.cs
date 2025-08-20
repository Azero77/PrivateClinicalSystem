using ClinicApp.Domain.Common;

namespace ClinicApp.Application.DataQueryHelpers;
public interface IPaginatedRepository<T>
    where T : Entity
{
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber, int itemsPerPage);
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber);
    Task<IReadOnlyCollection<T>> GetItems(DataQueryOptions<T> opts);
}
