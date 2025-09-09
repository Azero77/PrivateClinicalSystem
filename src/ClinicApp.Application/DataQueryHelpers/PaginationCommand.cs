using ClinicApp.Domain.Common;
using ErrorOr;

namespace ClinicApp.Application.DataQueryHelpers;

public class PaginationCommand<T> : IQueryCommand<T>
    where T : Entity
{
    private const int maxPageSize = 50;
    private readonly int _pageNumber;
    private readonly int _pageSize;

    private PaginationCommand(int pageNumber, int pageSize)
    {
        _pageNumber = pageNumber;
        _pageSize = pageSize;
    }

    public ErrorOr<PaginationCommand<T>> Create(int pageNumber, int pageSize)
    {
        if (pageSize > maxPageSize)
            return Error.Validation("Query.Validation", "Maximum pagesize is 50");
        return new PaginationCommand<T>(pageNumber,pageSize);
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Skip(_pageNumber * _pageSize)
            .Take(_pageSize);
    }
}
