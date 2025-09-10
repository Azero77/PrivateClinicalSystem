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

    public GetSessionQueryHandler()
    {
    }

    public async Task<ErrorOr<IReadOnlyCollection<Session>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        
        throw new Exception();
    }

}


