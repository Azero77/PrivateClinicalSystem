using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Infrastructure.QueryServices;

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