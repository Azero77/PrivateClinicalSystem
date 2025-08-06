
using ClinicApp.Domain.Common.Entities;

namespace ClinicApp.Domain.Repositories;

public interface IRoomRepository
{
    Task<IReadOnlyCollection<Room>> GetAllRooms();
    Task<Room?> GetRoomById(Guid roomId);
    Task<Room> GetRoomByDoctorId(Guid doctorId);
    Task<Room?> AddRoom(Room room);
    Task DeleteRoom(Room deletedRoom);
    Task<Room> UpdateRoom(Room updatedRoom);

}
