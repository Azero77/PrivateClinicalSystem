using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SecretaryAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SecretaryCommands;

public record DeleteSecretaryCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteSecretaryCommandHandler : IRequestHandler<DeleteSecretaryCommand, ErrorOr<Success>>
{
    private readonly ISecretaryRepository _secretaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSecretaryCommandHandler(ISecretaryRepository secretaryRepository, IUnitOfWork unitOfWork)
    {
        _secretaryRepository = secretaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteSecretaryCommand request, CancellationToken cancellationToken)
    {
        var deletedSecretary = await _secretaryRepository.DeleteSecretary(request.Id);
        if (deletedSecretary is null)
        {
            return Errors.General.NotFound;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken, deletedSecretary);
        return Result.Success;
    }
}
