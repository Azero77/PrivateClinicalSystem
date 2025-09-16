using ClinicApp.Application.Common;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.PatientCommands;

public record UpdatePatientCommand(Guid Id, string FirstName, string LastName) : IRequest<ErrorOr<Patient>>;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, ErrorOr<Patient>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Patient>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetById(request.Id);
        if (patient is null)
        {
            return Errors.General.NotFound;
        }

        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;

        var updatedPatient = await _patientRepository.UpdatePatient(patient);
        await _unitOfWork.SaveChangesAsync(cancellationToken, updatedPatient);
        return updatedPatient;
    }
}
