using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.RejectSessionsCommands;
public record RejectSessionCommand(Session Session) : IRequest<ErrorOr<Success>>;
