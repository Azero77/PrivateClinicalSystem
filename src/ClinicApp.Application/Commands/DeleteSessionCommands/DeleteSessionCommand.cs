using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.DeleteSessionCommands;
public record DeleteSessionCommand(Session Session) : IRequest<ErrorOr<Deleted>>;