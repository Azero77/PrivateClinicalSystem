using ClinicApp.Domain.Doctor;

namespace ClinicApp.Domain.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor.Doctor?> GetByIdAsync(Guid id);
        void Add(Doctor.Doctor doctor);
    }
}