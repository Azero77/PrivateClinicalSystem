using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Domain.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor> AddDoctor(Doctor doctor);
        Task<IReadOnlyCollection<Doctor>> GetDoctors();
        Task<Doctor?> GetDoctorByRoom(Guid roomId);
        Task<Doctor?> DeleteDoctor(Guid doctorId);
        Task<Doctor> UpdateDoctor(Doctor doctor);
    }
}