using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.ModifySession;

public abstract record ModifySessionCommand(Guid SessionId) : IRequest<ErrorOr<Success>>;
