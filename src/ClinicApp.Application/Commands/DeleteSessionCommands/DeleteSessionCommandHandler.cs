using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.DeleteSessionCommands;

public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, ErrorOr<Deleted>>
{
    public Task<ErrorOr<Deleted>> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.DeleteSession());
    }
}