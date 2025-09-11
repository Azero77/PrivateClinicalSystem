using ClinicApp.Application.Converters;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.AddSessionsCommands;
public record AddSessionCommand(
                       DateTimeOffset StartTime,
                       DateTimeOffset EndTime,
                       string SessionDescriptionContent,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       UserRole role) : IRequest<ErrorOr<Session>>;


