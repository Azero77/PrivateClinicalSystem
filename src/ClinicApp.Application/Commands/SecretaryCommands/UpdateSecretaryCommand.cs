using ClinicApp.Application.Common;
using ClinicApp.Domain.SecretaryAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SecretaryCommands;

public record UpdateSecretaryCommand(Guid Id, string FirstName, string LastName) : IRequest<ErrorOr<Secretary>>;

public class UpdateSecretaryCommandHandler : IRequestHandler<UpdateSecretaryCommand, ErrorOr<Secretary>>
{
    private readonly ISecretaryRepository _secretaryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSecretaryCommandHandler(ISecretaryRepository secretaryRepository, IUnitOfWork unitOfWork)
    {
        _secretaryRepository = secretaryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Secretary>> Handle(UpdateSecretaryCommand request, CancellationToken cancellationToken)
    {
        var secretary = await _secretaryRepository.GetById(request.Id);
        if (secretary is null)
        {
            return Errors.General.NotFound;
        }

        secretary.FirstName = request.FirstName;
        secretary.LastName = request.LastName;

        var updatedSecretary = await _secretaryRepository.UpdateSecretary(secretary);
        await _unitOfWork.SaveChangesAsync(cancellationToken, updatedSecretary);
        return updatedSecretary;
    }
}
