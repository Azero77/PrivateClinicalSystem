using ClinicApp.Application.DTOs;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries;
public record GetDoctorWithSessionsQuery(Guid doctorId) : IRequest<ErrorOr<DoctorWithSessionsDTO>>;
