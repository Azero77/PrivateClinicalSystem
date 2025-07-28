using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Domain.Repositories;

public interface IRoomRepository
{
    public Doctor GetDoctorOfRoom(Guid roomId);
}
