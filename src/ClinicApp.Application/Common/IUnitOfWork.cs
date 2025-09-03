using ClinicApp.Domain.Common;

namespace ClinicApp.Application.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default);
    void Track(AggregateRoot root);
}
