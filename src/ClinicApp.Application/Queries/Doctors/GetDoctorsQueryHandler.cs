using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Repositories;
using MediatR;

namespace ClinicApp.Application.Queries.Doctors;

public class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, IEnumerable<DoctorDTO>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorsQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<DoctorDTO>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {        
        var doctors = await _doctorRepository.GetDoctors();
        return doctors.Select(d => new DoctorDTO
        {
            Id = d.Id,
            UserId = d.UserId,
            FirstName = d.FirstName,
            LastName = d.LastName,
            Major = d.Major,
            RoomId = d.RoomId
        });
    }
}
