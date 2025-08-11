using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.RejectSessionsCommands;

public class RejectSessionCommandHandler : IRequestHandler<RejectSessionCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(RejectSessionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.RejectSession());
    }
}