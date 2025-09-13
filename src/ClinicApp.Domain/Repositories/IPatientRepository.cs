using ClinicApp.Domain.PatientAgg;

namespace ClinicApp.Domain.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient> AddPatient(Patient patient);
    Task<Patient> UpdatePatient(Patient patient);
    Task<Patient?> DeletePatient(Guid patientId);
    Task<Patient?> GetPatientByUserId(Guid userId);
    Task<IReadOnlyCollection<Patient>> GetPatients();
}
