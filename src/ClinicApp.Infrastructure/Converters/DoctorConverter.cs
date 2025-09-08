using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;

namespace ClinicApp.Infrastructure.Converters;

public class DoctorConverter : IConverter<Doctor, DoctorDataModel>
{
    public Doctor MapToEntity(DoctorDataModel model)
    {
        return new Doctor(
            model.Id,
            model.UserId,
            model.FirstName,
            model.LastName,
            model.RoomId,
            model.WorkingTime.WorkingDays,
            model.WorkingTime.WorkingHours,
            model.Major
        );
    }

    public DoctorDataModel MapToData(Doctor entity)
    {
        return new DoctorDataModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            RoomId = entity.RoomId,
            WorkingTime = entity.WorkingTime,
            Major = entity.Major
        };
    }
}
