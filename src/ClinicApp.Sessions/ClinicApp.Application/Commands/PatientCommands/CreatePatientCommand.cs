using ClinicApp.Application.Common;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.PatientCommands;

public record CreatePatientCommand(string FirstName, string LastName, Guid UserId) : IRequest<ErrorOr<Patient>>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, ErrorOr<Patient>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient(Guid.NewGuid(), request.UserId, request.FirstName, request.LastName);
        var createdPatient = await _patientRepository.AddPatient(patient);
        await _unitOfWork.SaveChangesAsync(cancellationToken, createdPatient);
        return createdPatient;
    }
}
