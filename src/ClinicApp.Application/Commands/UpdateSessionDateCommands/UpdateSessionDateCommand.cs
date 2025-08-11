using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.UpdateSessionDateCommands;
public record UpdateSessionDateCommand(Session Session, TimeRange NewTimeRange) : IRequest<ErrorOr<Success>>;