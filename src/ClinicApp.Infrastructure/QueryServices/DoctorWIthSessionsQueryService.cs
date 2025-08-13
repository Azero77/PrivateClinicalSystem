using ClinicApp.Application.DTOs;
using ClinicApp.Application.QueryServices;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClinicApp.Infrastructure.QueryServices;
public class DoctorQueryService : IDoctorQueryService
{
    private readonly AppDbContext _context;

    public DoctorQueryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DoctorWithSessionsDTO?> GetDoctorWithSessions(Guid doctorid)
    {
        var query = from d in _context.Doctors.AsNoTracking()
                    where d.Id == doctorid
                    join s in _context.Sessions.AsNoTracking() on d.Id equals s.DoctorId into doctorSessions
                    select new DoctorWithSessionsDTO
                    {
                        Id = d.Id,
                        Name = d.FirstName + " " + d.LastName,
                        WorkingDays = d.WorkingTime.WorkingDays,
                        WorkingHours = d.WorkingTime.WorkingHours,
                        Sessions = doctorSessions.ToList()
                    };

        return await query.FirstOrDefaultAsync();
    }
}
