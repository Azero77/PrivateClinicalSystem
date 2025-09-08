using ClinicApp.Application.DTOs;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries;

public class GetDoctorWithSessionsQueryHandler : IRequestHandler<GetDoctorWithSessionsQuery, ErrorOr<DoctorWithSessionsDTO>>
{
    ISessionRepository _sessionRepo;
    IDoctorRepository _doctorRepo;

    public GetDoctorWithSessionsQueryHandler(IDoctorRepository repo, ISessionRepository sessionRepo)
    {
        _doctorRepo = repo;
        _sessionRepo = sessionRepo;
    }

    public async Task<ErrorOr<DoctorWithSessionsDTO>> Handle(GetDoctorWithSessionsQuery request, CancellationToken cancellationToken)
    {

        Doctor? doctor = await _doctorRepo.GetById(request.doctorId);

        if (doctor is null)
            return Error.NotFound("Application.NotFound",
                "Doctor with this id is not found");

        var sessions = await _sessionRepo.GetAllSessionsForDoctor(doctor);
        return Converters.Converters.DTOFrom(doctor,sessions);
    }
}
