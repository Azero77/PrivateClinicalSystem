using ClinicApp.Domain.Common;

namespace ClinicApp.Infrastructure.DataQueryHelpers;

public class PaginationCommand<T> : IQueryCommand<T>
    where T : Entity
{
    private readonly int _pageNumber;
    private readonly int _pageSize;

     public PaginationCommand(int pageNumber, int pageSize)
    {
        _pageNumber = pageNumber;
        _pageSize = pageSize;
    }

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Skip(_pageNumber * _pageSize)
            .Take(_pageNumber);
    }
}
