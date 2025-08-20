using ClinicApp.Domain.Common;

namespace ClinicApp.Application.DataQueryHelpers;

public interface IQueryCommand<T>
    where T : Entity
{
    IQueryable<T> Apply(IQueryable<T> query);
}
