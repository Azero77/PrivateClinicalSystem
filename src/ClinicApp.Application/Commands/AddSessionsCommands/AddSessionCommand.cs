using ClinicApp.Application.Converters;
using ClinicApp.Application.QueryServices;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.AddSessionsCommands;
public record AddSessionCommand(
                        Guid sessionId,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       UserRole role) : IRequest<ErrorOr<Session>>;
