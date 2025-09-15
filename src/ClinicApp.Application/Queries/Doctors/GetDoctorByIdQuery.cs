using ClinicApp.Application.DTOs;
using MediatR;

namespace ClinicApp.Application.Queries.Doctors;

public record GetDoctorByIdQuery(Guid Id) : IRequest<DoctorDTO?>;
