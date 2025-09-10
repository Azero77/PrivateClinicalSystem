using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.QueryServices;

public class SessionQueryService(AppDbContext context) : IQueryService<SessionQueryType>
{

    public IQueryable<SessionQueryType> GetItemById(Guid id)
    {
        return context.Sessions
            .Where(e => e.Id == id)
            .Select(e => ConvertToQueryType(e));
    }

    public IQueryable<SessionQueryType> GetItems()
    {
        return context.Sessions.Select(e => ConvertToQueryType(e));
    }

    private static SessionQueryType ConvertToQueryType(SessionDataModel e)
    {
        return new SessionQueryType
        {
            SessionDate = e.SessionDate,
            SessionDescription = e.SessionDescription,
            RoomId = e.RoomId,
            Room = new RoomQueryType
            {
                Id = e.Room.Id,
                Name = e.Room.Name
            },
            PatientId = e.PatientId,
            Patient = new PatientQueryType
            {
                Id = e.Patient.Id,
                FirstName = e.Patient.FirstName,
                LastName = e.Patient.LastName
            },
            DoctorId = e.DoctorId,
            Doctor = new DoctorQueryType
            {
                Id = e.Doctor.Id,
                FirstName = e.Doctor.FirstName,
                LastName = e.Doctor.LastName,
                Major = e.Doctor.Major,
                RoomId = e.Doctor.RoomId,
                UserId = e.Doctor.UserId,
                WorkingTime = e.Doctor.WorkingTime
            },
            SessionStatus = e.SessionStatus,
            CreatedAt = e.CreatedAt
        };
    }
}