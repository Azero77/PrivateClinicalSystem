using ClinicApp.Domain.Common;

namespace ClinicApp.Infrastructure.Repositories;

internal interface IRepository<T, TData>
    where T: Entity
    where TData : class 
{
    Task<T?> GetById(Guid id);
    Task Save(T entity);
}

