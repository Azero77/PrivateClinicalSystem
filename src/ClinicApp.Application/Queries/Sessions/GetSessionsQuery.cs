using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionsQuery(
    string? DoctorId,
    DateTimeOffset? FromDatetime,
    DateTimeOffset? ToDateTime,
    string? roomId,
    string? patientId,
    SessionStatus? status,
    int pageNumber,
    int pageSize,
    string[] sortOptions //would look something like this ?sortOptions=startTime:ASC
                                ) : IRequest<ErrorOr<IReadOnlyCollection<Session>>>;


public class GetSessionQueryHandler : IRequestHandler<GetSessionsQuery, ErrorOr<IReadOnlyCollection<Session>>>
{
    private readonly IPaginatedRepository<Session> _repo;

    public GetSessionQueryHandler(IPaginatedRepository<Session> repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<IReadOnlyCollection<Session>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        //we need to validate each filter command to have the type matched
        //for order command
        throw new Exception();
    }
}
