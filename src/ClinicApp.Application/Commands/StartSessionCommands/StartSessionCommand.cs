using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.StartSessionCommands;
public record StartSessionCommand(Session Session) : IRequest<ErrorOr<Success>>;