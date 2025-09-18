
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ClinicApp.Infrastructure.Repositories;

public class CachedDbRoomRepository : Repository<Room, RoomDataModel>, IRoomRepository
{
    private readonly DbRoomRepository _repo;
    private readonly IDistributedCache _cache;

    public CachedDbRoomRepository(AppDbContext context,
                                    IConverter<Room, RoomDataModel> converter,
                                    DbRoomRepository repo,
                                    IDistributedCache cache) : base(context, converter)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Room?> AddRoom(Room room)
    {
        var newRoom = await _repo.AddRoom(room);
        if (newRoom is null)
            return null;

        string key = $"room-{newRoom.Id}";
        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(_converter.MapToData(newRoom)));
        await _cache.RemoveAsync("rooms");
        return newRoom;
    }

    public async Task<Room?> DeleteRoom(Guid roomId)
    {
        var deletedRoom = await _repo.DeleteRoom(roomId);
        if (deletedRoom is null)
            return null;

        string key = $"room-{roomId}";
        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync("rooms");
        return deletedRoom;
    }

    public async Task<IReadOnlyCollection<Room>> GetAllRooms()
    {
        string key = "rooms";
        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            var rooms = await _repo.GetAllRooms();
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(rooms.Select(r => _converter.MapToData(r))));
            return rooms;
        }

        var roomDataModels = JsonConvert.DeserializeObject<List<RoomDataModel>>(json,
            new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

        if (roomDataModels is null)
            return new List<Room>().AsReadOnly();

        return roomDataModels.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<Room> UpdateRoom(Room updatedRoom)
    {
        var room = await _repo.UpdateRoom(updatedRoom);
        string key = $"room-{room.Id}";
        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync("rooms");
        return room;
    }
}
