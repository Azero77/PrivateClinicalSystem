using ClinicApp.Domain.Common;
using System.Linq.Expressions;

namespace ClinicApp.Infrastructure.DataQueryHelpers;

public class FilterCommand<T> : IQueryCommand<T>
    where T : Entity
{
    public FilterCommand(Expression<Func<T,bool>> filter)
    {
        Filter = filter;
    }

    public Expression<Func<T, bool>> Filter { get; }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Where(Filter);
    }
}
