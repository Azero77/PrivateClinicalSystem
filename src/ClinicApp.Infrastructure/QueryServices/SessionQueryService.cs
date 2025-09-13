using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClinicApp.Infrastructure.QueryServices;

public class SessionQueryService(AppDbContext context) : IQueryService<SessionQueryType>
{

    public async Task<SessionQueryType?> GetItemById(Guid id)
    {
        return await context.Sessions
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e =>
                 new SessionQueryType
                {
                    Id = e.Id,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Content = e.Content,
                    RoomId = e.RoomId,
                    Room = e.Room == null ? null : new RoomQueryType
                    {
                        Id = e.Room.Id,
                        Name = e.Room.Name
                    },
                    PatientId = e.PatientId,
                    Patient = e.Patient == null ? null : new PatientQueryType
                    {
                        Id = e.Patient.Id,
                        FirstName = e.Patient.FirstName,
                        LastName = e.Patient.LastName,
                        UserId = e.Patient.UserId
                    },
                    DoctorId = e.DoctorId,
                    Doctor = e.Doctor == null ? null : new DoctorQueryType
                    {
                        Id = e.Doctor.Id,
                        FirstName = e.Doctor.FirstName,
                        LastName = e.Doctor.LastName,
                        Major = e.Doctor.Major,
                        RoomId = e.Doctor.RoomId,
                        UserId = e.Doctor.UserId,
                        WorkingDays = e.Doctor.WorkingDays,
                        StartTime = e.Doctor.StartTime,
                        EndTime = e.Doctor.EndTime,
                        TimeZoneId = e.Doctor.TimeZoneId,
                        TimesOff = e.Doctor.TimesOff.Select(t => new TimeOffQueryType { StartDate = t.StartDate, EndDate = t.EndDate, reason = t.reason }).ToList()
                    },
                    SessionStatus = e.SessionStatus,
                    CreatedAt = e.CreatedAt
                }
            ).SingleOrDefaultAsync();
    }

    public IQueryable<SessionQueryType> GetItems()
    {
        return context.Sessions.AsNoTracking().Select(e =>
                 new SessionQueryType
                 {
                     Id = e.Id,
                     StartTime = e.StartTime,
                     EndTime = e.EndTime,
                     Content = e.Content,
                     RoomId = e.RoomId,
                     Room = e.Room == null ? null : new RoomQueryType
                     {
                         Id = e.Room.Id,
                         Name = e.Room.Name
                     },
                     PatientId = e.PatientId,
                     Patient = e.Patient == null ? null : new PatientQueryType
                     {
                         Id = e.Patient.Id,
                         FirstName = e.Patient.FirstName,
                         LastName = e.Patient.LastName,
                         UserId = e.Patient.UserId
                     },
                     DoctorId = e.DoctorId,
                     Doctor = e.Doctor == null ? null : new DoctorQueryType
                     {
                         Id = e.Doctor.Id,
                         FirstName = e.Doctor.FirstName,
                         LastName = e.Doctor.LastName,
                         Major = e.Doctor.Major,
                         RoomId = e.Doctor.RoomId,
                         UserId = e.Doctor.UserId,
                         WorkingDays = e.Doctor.WorkingDays,
                         StartTime = e.Doctor.StartTime,
                         EndTime = e.Doctor.EndTime,
                         TimeZoneId = e.Doctor.TimeZoneId,
                         TimesOff = e.Doctor.TimesOff.Select(t => new TimeOffQueryType { StartDate = t.StartDate, EndDate = t.EndDate, reason = t.reason }).ToList()
                     },
                     SessionStatus = e.SessionStatus,
                     CreatedAt = e.CreatedAt
                 }
            );
    }

}
