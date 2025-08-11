using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.FinishSessionCommands;
public record FinishSessionCommand(Session Session) : IRequest<ErrorOr<Success>>;