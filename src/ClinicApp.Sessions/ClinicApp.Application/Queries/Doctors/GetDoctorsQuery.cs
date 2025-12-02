using ClinicApp.Application.DTOs;
using MediatR;

namespace ClinicApp.Application.Queries.Doctors;

public record GetDoctorsQuery() : IRequest<IEnumerable<DoctorDTO>>;
