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
        var room = new Room();

        // The domain model has protected/private setters. Reflection is used here
        // to set the properties, similar to how an ORM like EF Core would materialize the object.
        var idProperty = typeof(Entity).GetProperty(nameof(room.Id));
        idProperty?.SetValue(room, roomData.Id);

        var nameProperty = typeof(Room).GetProperty(nameof(room.Name));
        if (nameProperty != null)
        {
            var nameBackingField = typeof(Room).GetField($"<{nameProperty.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            nameBackingField?.SetValue(room, roomData.Name);
        }

        return room;
    }
}
