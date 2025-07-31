using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.DoctorAgg;
using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Domain.AdminAggregate
{
    public class Admin : AggregateRoot
    {
        public List<Room> RoomIds { get; private set; } = new();

        public Guid UserId {get;private set;}

        public ErrorOr<Success> AddRoom(Room room)
        {
            if (RoomIds.Contains(room))
            {
                return AdminErrors.RoomAlreadyExists;
            }

            RoomIds.Add(room);
            _domainEvents.Add(new AdminCreatedRoomEvent(room));
            return Result.Success;
        }

        public ErrorOr<Deleted> RemoveRoom(Room room)
        {
            if (!RoomIds.Contains(room))
            {
                return AdminErrors.RoomNotFound;
            }

            RoomIds.Remove(room);
            _domainEvents.Add(new AdminDeletedRoomEvent(room));
            return Result.Deleted;
        }


    }

    public record AdminCreatedRoomEvent(Room room) : IDomainEvent;
    public record AdminDeletedRoomEvent(Room room) : IDomainEvent;
}
