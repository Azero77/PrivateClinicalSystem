using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Doctors;

public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, ErrorOr<DoctorDTO>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<ErrorOr<DoctorDTO>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetById(request.Id);
        if (doctor is null)
            return Application.Common.Errors.General.NotFound;

        return new DoctorDTO
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Major = doctor.Major,
            RoomId = doctor.RoomId
        };
    }
}
