using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;
using System.Windows.Input;
using System.Windows.Markup;

namespace ClinicApp.Application.Queries.Sessions;
public record GetSessionsQuery(
    string? FilterDoctorId,
    DateTimeOffset? FilterFromDatetime,
    DateTimeOffset? FilterToDateTime,
    string? FilterRoomId,
    string? FilterPatientId,
    SessionStatus? FilterStatus,
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
        //for order command we create the order command and see if there is errors

        List<ICommand> commands = new();
        var properties = request.GetType()
            .GetProperties()
            .Where(p => p.GetValue(request) is not null);
        throw new Exception();
    }

}


