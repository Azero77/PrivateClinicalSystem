using ClinicApp.Application.DTOs;
using MediatR;

namespace ClinicApp.Application.Queries;
public record GetDoctorWithSessionsQuery(Guid doctorId) : IRequest<DoctorWithSessionsDTO>;
