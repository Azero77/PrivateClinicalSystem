using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbRoomRepository : Repository<Room,RoomDataModel>,IRoomRepository
{
    public DbRoomRepository(AppDbContext context, IConverter<Room, RoomDataModel> converter) : base(context, converter)
    {
    }

    public async Task<Room?> AddRoom(Room room)
    {
        var roomData = _converter.MapToData(room);
        await _context.Rooms.AddAsync(roomData);
        return room;
    }

    public Task DeleteRoom(Room deletedRoom)
    {
        var roomData = _converter.MapToData(deletedRoom);
        _context.Rooms.Remove(roomData);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyCollection<Room>> GetAllRooms()
    {
        var roomsData = await _context.Rooms.AsNoTracking().ToListAsync();
        return roomsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Room> UpdateRoom(Room updatedRoom)
    {
        var roomData = _converter.MapToData(updatedRoom);
        _context.Rooms.Update(roomData);
        return Task.FromResult(updatedRoom);
    }
}