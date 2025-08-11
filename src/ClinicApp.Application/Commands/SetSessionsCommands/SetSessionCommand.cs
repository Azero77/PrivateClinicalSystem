using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SetSessionsCommands;
public record SetSessionCommand(Session Session) : IRequest<ErrorOr<Success>>;
