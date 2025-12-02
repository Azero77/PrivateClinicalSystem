using ClinicApp.Application.DTOs;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Doctors;

public record GetDoctorByIdQuery(Guid Id) : IRequest<ErrorOr<DoctorDTO>>;
