using ClinicApp.Application.Common;
using ClinicApp.Domain.SecretaryAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SecretaryCommands;

public record CreateSecretaryCommand(string FirstName, string LastName, Guid UserId) : IRequest<ErrorOr<Secretary>>;

public class CreateSecretaryCommandHandler : IRequestHandler<CreateSecretaryCommand, ErrorOr<Secretary>>
{
    private readonly ISecretaryRepository _secretaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSecretaryCommandHandler(ISecretaryRepository secretaryRepository, IUnitOfWork unitOfWork)
    {
        _secretaryRepository = secretaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Secretary>> Handle(CreateSecretaryCommand request, CancellationToken cancellationToken)
    {
        var secretary = new Secretary(Guid.NewGuid(), request.UserId, request.FirstName, request.LastName);
        var createdSecretary = await _secretaryRepository.AddSecretary(secretary);
        await _unitOfWork.SaveChangesAsync(cancellationToken, createdSecretary);
        return createdSecretary;
    }
}
