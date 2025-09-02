using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Mappers;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbDoctorRepository : PaginatedRepository<Doctor>,IDoctorRepository
{
    private readonly AppDbContext _context;
    private const int InitialPageNumber = 5;


    public DbDoctorRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }
    public async Task<Doctor?> DeleteDoctor(Guid doctorId)
    {
        DoctorDataModel? doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor is not null)
        {
            _context.Remove(doctor);
        }

        return doctor?.ToDomain();
    }

    public async Task<Doctor?> GetByIdAsync(Guid id)
    {
        DoctorDataModel? doctorDataModel = await _context.Doctors.AsNoTracking().SingleOrDefaultAsync(d => d.Id == id);
        return doctorDataModel?.ToDomain();
    }
    public async Task<Doctor?> GetDoctorByRoom(Guid roomId)
    {
        return (await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.RoomId == roomId))?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Doctor>> GetDoctors()
    {
        var list = await _context.Doctors.AsNoTracking().Select(d => d.ToDomain()).ToListAsync();
        
        return list.AsReadOnly();
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        
        _context.Doctors.Update(doctor.ToDataModel());
        return Task.FromResult(doctor);
    }
    public async Task<Doctor?> AddDoctor(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor.ToDataModel());
        return doctor;
    }
}
