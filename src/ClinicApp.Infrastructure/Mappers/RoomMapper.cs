using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Infrastructure.Persistance.DataModels;
using System.Reflection;

namespace ClinicApp.Infrastructure.Mappers;

public static class RoomMapper
{
    public static RoomDataModel ToDataModel(this Room room)
    {
        return new RoomDataModel
        {
            Id = room.Id,
            Name = room.Name
        };
    }

    public static Room ToDomain(this RoomDataModel roomData)
    {
        return new Room(roomData.Id,roomData.Name);
    }
}
