using ClinicApp.Application.DTOs;
using MediatR;

namespace ClinicApp.Application.Queries;

public class GetDoctorWithSessionsQueryHandler : IRequestHandler<GetDoctorWithSessionsQuery, DoctorWithSessionsDTO>
{
    IDoctorQueryService _service;

    public GetDoctorWithSessionsQueryHandler(IDoctorQueryService service)
    {
        _service = service;
    }

    public Task<DoctorWithSessionsDTO> Handle(GetDoctorWithSessionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
