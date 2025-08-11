using ClinicApp.Application.DTOs;
using ClinicApp.Application.QueryServices;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries;

public class GetDoctorWithSessionsQueryHandler : IRequestHandler<GetDoctorWithSessionsQuery, ErrorOr<DoctorWithSessionsDTO>>
{
    IDoctorQueryService _service;

    public GetDoctorWithSessionsQueryHandler(IDoctorQueryService service)
    {
        _service = service;
    }

    public async Task<ErrorOr<DoctorWithSessionsDTO>> Handle(GetDoctorWithSessionsQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _service.GetDoctorWithSessions(request.doctorId);

        return doctor is null ? Error.NotFound() : doctor;
    }
}
