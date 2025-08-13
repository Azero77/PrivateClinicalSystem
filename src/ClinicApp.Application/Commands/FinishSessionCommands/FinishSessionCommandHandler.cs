using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.FinishSessionCommands;

public class FinishSessionCommandHandler : IRequestHandler<FinishSessionCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(FinishSessionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.FinishSession());
    }
}