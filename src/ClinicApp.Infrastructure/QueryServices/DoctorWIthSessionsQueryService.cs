using ClinicApp.Application.DTOs;
using ClinicApp.Application.QueryServices;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

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
        DoctorWithSessionsDTO? doctordto = await _context.Doctors.Where(d => d.Id == doctorid)
            .Select(d => new DoctorWithSessionsDTO()
            {
                Id = d.Id,
                Name = d.FirstName + " " + d.LastName,
                Sessions = _context.Sessions.AsNoTracking().Where(s => s.DoctorId == doctorid).ToList()
            })
            .FirstOrDefaultAsync();

        return doctordto;
    }
}
