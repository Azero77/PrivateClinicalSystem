using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Domain.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(Guid id);
        void Add(Doctor doctor);
    }
}