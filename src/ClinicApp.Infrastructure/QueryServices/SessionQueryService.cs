using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

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

public class DoctorQueryService(AppDbContext context) : IQueryService<DoctorQueryType>
{
    private Expression<Func<DoctorDataModel, DoctorQueryType>> converter = d => new DoctorQueryType()
    {
        Id = d.Id,
        FirstName = d.FirstName,
        LastName = d.LastName,

        Major = d.Major,
        RoomId = d.RoomId,
        Room = d.Room == null ? null : new RoomQueryType
        {
            Id = d.Room.Id,
            Name = d.Room.Name
        },
        Sessions = d.Sessions == null ? null : d.Sessions.Select(s => new SessionQueryType
        {
            Id = s.Id,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            PatientId = s.PatientId,
            DoctorId = s.DoctorId,
            RoomId = s.RoomId,
            Content = s.Content,
            CreatedAt = s.CreatedAt,
            SessionStatus = s.SessionStatus,
        }).ToList(),
        WorkingDays = d.WorkingDays,
        StartTime = d.StartTime,
        EndTime = d.EndTime,
        TimeZoneId = d.TimeZoneId,
        TimesOff = d.TimesOff.Select(t => new TimeOffQueryType
        {

            StartDate = t.StartDate,
            EndDate = t.EndDate,
            reason = t.reason
        }).ToList()
    };
    public async Task<DoctorQueryType?> GetItemById(Guid id)
    {
        return await context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(converter)
            .SingleOrDefaultAsync();
    }

    public IQueryable<DoctorQueryType> GetItems()
    {

        Expression<Func<DoctorDataModel, DoctorQueryType>> converter = d => new DoctorQueryType()
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,

            Major = d.Major,
            RoomId = d.RoomId,
            Room = d.Room == null ? null : new RoomQueryType
            {
                Id = d.Room.Id,
                Name = d.Room.Name
            },
            Sessions = d.Sessions == null ? null : d.Sessions.Select(s => new SessionQueryType
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                PatientId = s.PatientId,
                DoctorId = s.DoctorId,
                RoomId = s.RoomId,
                Content = s.Content,
                CreatedAt = s.CreatedAt,
                SessionStatus = s.SessionStatus,
            }).ToList(),
            WorkingDays = d.WorkingDays,
            StartTime = d.StartTime,
            EndTime = d.EndTime,
            TimeZoneId = d.TimeZoneId,
            TimesOff = d.TimesOff.Select(t => new TimeOffQueryType
            {

                StartDate = t.StartDate,
                EndDate = t.EndDate,
                reason = t.reason
            }).ToList()
        };
        return context.Doctors.AsNoTracking().Select(converter);
    }
}