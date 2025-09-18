using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ErrorOr;

namespace ClinicApp.Domain.AdminAggregate;

public class Admin : Member
{
    public List<Room> Rooms { get; private set; } = [];

    public ErrorOr<Success> AddRoom(Room room)
    {
        if (Rooms.Contains(room))
        {
            return AdminErrors.RoomAlreadyExists;
        }

        Rooms.Add(room);
        _domainEvents.Add(new AdminCreatedRoomEvent(room));
        return Result.Success;
    }

    public ErrorOr<Deleted> RemoveRoom(Room room)
    {
        if (!Rooms.Contains(room))
        {
            return AdminErrors.RoomNotFound;
        }

        Rooms.Remove(room);
        _domainEvents.Add(new AdminDeletedRoomEvent(room));
        return Result.Deleted;
    }


}

public record AdminCreatedRoomEvent(Room Room) : IDomainEvent;
public record AdminDeletedRoomEvent(Room Room) : IDomainEvent;
