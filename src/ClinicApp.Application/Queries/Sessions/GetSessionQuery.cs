using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionQuery(Guid id) : IRequest<IQueryable<SessionQueryType>>;
public sealed class GetSessionQueryHandler(IQueryService<SessionQueryType> queryService)
    : IRequestHandler<GetSessionQuery, IQueryable<SessionQueryType>>
{
    public Task<IQueryable<SessionQueryType>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(queryService.GetItemById(request.id));
    }
}