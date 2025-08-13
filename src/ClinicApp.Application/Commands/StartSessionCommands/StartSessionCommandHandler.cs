using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.StartSessionCommands;

public class StartSessionCommandHandler : IRequestHandler<StartSessionCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.StartSession());
    }
}