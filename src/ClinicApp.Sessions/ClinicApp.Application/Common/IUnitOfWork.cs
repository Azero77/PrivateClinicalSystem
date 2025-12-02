using ClinicApp.Domain.Common;

namespace ClinicApp.Application.Common;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default,params AggregateRoot[] entities);
}
