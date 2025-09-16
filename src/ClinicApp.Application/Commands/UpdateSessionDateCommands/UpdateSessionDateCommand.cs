using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.UpdateSessionDateCommands;
public record UpdateSessionDateCommand(Guid SessionId, DateTimeOffset StartTime,DateTimeOffset EndTime) : IRequest<ErrorOr<Success>>;

