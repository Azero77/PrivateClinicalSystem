using ClinicApp.Domain.Common;

namespace ClinicApp.Infrastructure.Repositories;

internal interface IRepository<T, TData>
    where T: AggregateRoot
    where TData : class 
{
    T GetById(Guid id);
    Task Save(T entity);
}

