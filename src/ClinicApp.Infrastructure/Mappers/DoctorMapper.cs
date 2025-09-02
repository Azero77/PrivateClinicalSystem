using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;

namespace ClinicApp.Infrastructure.Mappers;

public static class DoctorMapper
{
    public static DoctorDataModel ToDataModel(this Doctor doctor)
    {
        return new DoctorDataModel
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            WorkingTime = doctor.WorkingTime,
            Major = doctor.Major,
            RoomId = doctor.RoomId
        };
    }

    public static Doctor ToDomain(this DoctorDataModel doctorData)
    {
        // The Doctor domain object has a public constructor that can be used for mapping.
        var doctor = new Doctor(
            doctorData.Id,
            doctorData.WorkingTime.WorkingDays,
            doctorData.WorkingTime.WorkingHours,
            doctorData.RoomId,
            doctorData.UserId,
            doctorData.FirstName,
            doctorData.LastName,
            doctorData.Major);

        // Set properties from the base Member class
        

        return doctor;
    }
}