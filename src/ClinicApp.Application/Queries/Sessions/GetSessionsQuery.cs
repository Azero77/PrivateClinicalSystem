using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionsQuery() : IRequest<ErrorOr<IReadOnlyCollection<SessionQueryType>>>;


public class GetSessionQueryHandler : IRequestHandler<GetSessionsQuery, ErrorOr<IReadOnlyCollection<Session>>>
{
    public GetSessionQueryHandler()
    {
    }

    public async Task<ErrorOr<IReadOnlyCollection<Session>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        
        throw new Exception();
    }

}


