using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionsQuery() : IRequest<IQueryable<SessionQueryType>>;


public class GetSessionQueryHandler(IQueryService<SessionQueryType> queryService) : IRequestHandler<GetSessionsQuery, IQueryable<SessionQueryType>>
{
    public Task<IQueryable<SessionQueryType>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(queryService.GetItems());
    }
}


