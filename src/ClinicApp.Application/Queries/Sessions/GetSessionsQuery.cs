using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionsQuery(DataQueryOptions<Session> QueryOpts) : IRequest<ErrorOr<IReadOnlyCollection<Session>>>;


public class GetSessionQueryHandler : IRequestHandler<GetSessionsQuery, ErrorOr<IReadOnlyCollection<Session>>>
{
    private readonly IPaginatedRepository<Session> _repo;

    public GetSessionQueryHandler(IPaginatedRepository<Session> repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<IReadOnlyCollection<Session>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Session>? result = await _repo.GetItems(request.QueryOpts);

        if (result is null)
            return Error.Validation();

        return result.ToErrorOr();
    }
}
