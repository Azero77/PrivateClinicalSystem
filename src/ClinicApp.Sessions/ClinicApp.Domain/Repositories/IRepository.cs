using ClinicApp.Domain.Common;

namespace ClinicApp.Domain.Repositories;

public interface IRepository<T>
    where T : AggregateRoot
{
    Task<T?> GetById(Guid id);
    Task SaveAsync(T Entity);
}


