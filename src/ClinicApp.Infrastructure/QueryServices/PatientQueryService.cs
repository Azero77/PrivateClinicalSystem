using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicApp.Infrastructure.QueryServices;

public class PatientQueryService(AppDbContext context) : IQueryService<PatientQueryType>
{
    private static Expression<Func<PatientDataModel, PatientQueryType>> _converter = p => new()
    {
        FirstName = p.FirstName,
        LastName = p.LastName,
        Id = p.Id,
        UserId = p.Id,
        Sessions = p.Sessions == null ? null : p.Sessions.Select(s => new SessionQueryType
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
        }).ToList()
    };
    public async Task<PatientQueryType?> GetItemById(Guid id)
    {
        return await context.Patients.Where(p => p.Id == id)
            .Select(_converter)
            .SingleOrDefaultAsync();
    }

    public IQueryable<PatientQueryType> GetItems()
    {
        return context.Patients.Select(_converter);
    }
}