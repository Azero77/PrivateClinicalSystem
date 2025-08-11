using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SetSessionsCommands;

public class SetSessionCommandHandler : IRequestHandler<SetSessionCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(SetSessionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.SetSession());
    }
}
