using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Domain.Common.Entities;

namespace ClinicApp.Infrastructure.Converters;

public class RoomConverter : IConverter<Room, RoomDataModel>
{
    public Room MapToEntity(RoomDataModel model)
    {
        return new Room(model.Id, model.Name);
    }

    public RoomDataModel MapToData(Room entity)
    {
        return new RoomDataModel
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}
