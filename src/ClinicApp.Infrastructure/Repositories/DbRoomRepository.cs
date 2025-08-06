using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbRoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public DbRoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> AddRoom(Room room)
    {
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task DeleteRoom(Room deletedRoom)
    {
        _context.Rooms.Remove(deletedRoom);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Room>> GetAllRooms()
    {
        return await _context.Rooms.AsNoTracking().ToListAsync();
    }

    public async Task<Room?> GetRoomById(Guid roomId)
    {
        return await _context.Rooms.SingleOrDefaultAsync(r => r.Id == roomId);
    }
    public async Task<Room> UpdateRoom(Room updatedRoom)
    {
        _context.Rooms.Update(updatedRoom);
        await _context.SaveChangesAsync();
        return updatedRoom;
    }
}