using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.UpdateSessionDateCommands;

public class UpdateSessionDateCommandHandler : IRequestHandler<UpdateSessionDateCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> Handle(UpdateSessionDateCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Session.UpdateDate(request.NewTimeRange));
    }
}