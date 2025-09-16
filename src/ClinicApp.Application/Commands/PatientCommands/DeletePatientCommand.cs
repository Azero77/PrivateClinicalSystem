using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.PatientCommands;

public record DeletePatientCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, ErrorOr<Success>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var deletedPatient = await _patientRepository.DeletePatient(request.Id);
        if (deletedPatient is null)
        {
            return Errors.General.NotFound;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken, deletedPatient);
        return Result.Success;
    }
}
