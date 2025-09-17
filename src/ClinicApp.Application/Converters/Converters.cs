using ClinicApp.Application.Commands.DoctorAddCommands;
using ClinicApp.Application.DTOs;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using Microsoft.AspNetCore.Http;

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

    public static SessionDTO FromSessionToDTO(this Session session)
    {
        return new SessionDTO()
        {
            Id = session.Id,
            DoctorId = session.DoctorId,
            PatientId = session.PatientId,
            RoomId = session.RoomId,
            StartTime = session.SessionDate.StartTime,
            EndTime = session.SessionDate.EndTime,
             SessionDescription = session.SessionDescription?.content ?? string.Empty,
             SessionStatus = session.SessionStatus

        };
    }

    public static DoctorDTO FromDoctorToDTO(this Doctor doctor)
    {
        return new()
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Major = doctor.Major,
            RoomId = doctor.RoomId,
            UserId  = doctor.UserId
        };
    }
}
