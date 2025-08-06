using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Domain.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(Guid doctorId);
        Task Add(Doctor doctor);
        Task<IReadOnlyCollection<Doctor>> GetDoctors();
        Task<Doctor?> GetDoctorByRoom(Guid roomId);
        Task DeleteDoctor(Guid doctorId);
        Task UpdateDoctor(Doctor doctor);
    }
}