namespace ClinicApp.Domain.Repositories;

public interface IRoomRepository
{
    public Doctor.Doctor GetDoctorOfRoom(Guid roomId);
}
