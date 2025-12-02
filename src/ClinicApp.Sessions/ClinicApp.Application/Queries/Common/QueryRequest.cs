using ClinicApp.Application.QueryServices;
using MediatR;

namespace ClinicApp.Application.Queries.Common;
public record QueryRequest<T>
    : IRequest<IQueryable<T>>
    where T : class;

public class QueryRequestHandler<T>(IQueryService<T> queryService) : IRequestHandler<QueryRequest<T>, IQueryable<T>>
    where T : class
{
    public Task<IQueryable<T>> Handle(QueryRequest<T> request, CancellationToken cancellationToken)
    {
        return Task.FromResult(queryService.GetItems());
    }
}


public record QuerySingleRequest<T>(Guid id)
    : IRequest<T?>
    where T : class;

public class QuerySingleRequestHandler<T>(IQueryService<T> queryService) : IRequestHandler<QuerySingleRequest<T>, T?>
    where T : class
{
    public Task<T?> Handle(QuerySingleRequest<T> request, CancellationToken cancellationToken)
    {
        return queryService.GetItemById(request.id);
    }
}