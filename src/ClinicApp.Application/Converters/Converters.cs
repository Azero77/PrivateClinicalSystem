using ClinicApp.Application.Commands.DoctorAddCommands;
using ClinicApp.Application.DTOs;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Application.Converters;
public static class Converters
{

    public static Doctor ToDoctor(this DoctorAddCommand doctordto)
    {
        return new Doctor(doctordto.doctorId,
            doctordto.userId,
            doctordto.firstName,
            doctordto.lastName,
            doctordto.roomId,
            doctordto.workingDays,
            WorkingHours.Create(doctordto.workingHoursStartTime,
                                doctordto.workingHoursEndTime).Value,
            doctordto.Major);
    }

    public static DoctorWithSessionsDTO DTOFrom(Doctor doctor,IReadOnlyCollection<Session> sessions)
    {
        return new DoctorWithSessionsDTO
        {
            Id = doctor.Id,
            Name = doctor.FirstName + " " + doctor.LastName,
            WorkingDays = doctor.WorkingTime.WorkingDays.ToListDays(),
            WorkingDaysBit = doctor.WorkingTime.WorkingDays.ToList(),
            WorkingHours = doctor.WorkingTime.WorkingHours,
            Sessions = sessions
        };
    }

}
