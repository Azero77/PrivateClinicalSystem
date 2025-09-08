using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbRoomRepository : PaginatedRepository<Room>,IRoomRepository,IRepository<Room,RoomDataModel>
{
    private readonly AppDbContext _context;

    public DbRoomRepository(AppDbContext context)
        :  base(context)
    {
        _context = context;
    }
    public async Task<Room?> GetById(Guid roomId)
    {
        return await _context.Rooms.SingleOrDefaultAsync(r => r.Id == roomId);
    }

    public Task Save(Room entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Room?> AddRoom(Room room)
    {
        await _context.Rooms.AddAsync(room);
        return room;
    }

    //TODO make the method take an id and ExecuteDeleteAsync() instead of the blocking one
    public Task DeleteRoom(Room deletedRoom)
    {
        _context.Rooms.Remove(deletedRoom);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyCollection<Room>> GetAllRooms()
    {
        return await _context.Rooms.AsNoTracking().ToListAsync();
    }

    public Task<Room> UpdateRoom(Room updatedRoom)
    {
        _context.Rooms.Update(updatedRoom);
        return Task.FromResult(updatedRoom);
    }
}