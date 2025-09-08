using ClinicApp.Domain.Common.Entities;

namespace ClinicApp.Domain.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<IReadOnlyCollection<Room>> GetAllRooms();
    Task<Room?> AddRoom(Room room);
    Task DeleteRoom(Room deletedRoom);
    Task<Room> UpdateRoom(Room updatedRoom);
}
