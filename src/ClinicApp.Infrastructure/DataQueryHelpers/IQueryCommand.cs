using ClinicApp.Domain.Common;

namespace ClinicApp.Infrastructure.DataQueryHelpers;

public interface IQueryCommand<T>
    where T : Entity
{
    IQueryable<T> Apply(IQueryable<T> query);
}
