using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;

namespace ClinicApp.Infrastructure.Converters;

public class DoctorConverter : IConverter<Doctor, DoctorDataModel>
{
    public Doctor MapToEntity(DoctorDataModel model)
    {
        var workingHours = WorkingHours.Create(model.StartTime, model.EndTime, model.TimeZoneId).Value;
        var doctor = new Doctor(
            model.Id,
            model.UserId,
            model.FirstName,
            model.LastName,
            model.RoomId,
            model.WorkingDays,
            workingHours,
            model.Major
        );

        foreach (var timeOff in model.TimesOff)
        {
            doctor.AddTimeOff(new TimeOff(timeOff.StartDate, timeOff.EndDate, timeOff.reason));
        }

        return doctor;
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
            Major = entity.Major,
            WorkingDays = entity.WorkingTime.WorkingDays,
            StartTime = entity.WorkingTime.WorkingHours.StartTime,
            EndTime = entity.WorkingTime.WorkingHours.EndTime,
            TimeZoneId = entity.WorkingTime.WorkingHours.TimeZoneId,
            TimesOff = entity.WorkingTime.TimesOff.Select(t => new TimeOffDataModel { StartDate = t.StartDate, EndDate = t.EndDate, reason = t.reason }).ToList()
        };
    }
}
