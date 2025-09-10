using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbDoctorRepository : Repository<Doctor,DoctorDataModel>,IDoctorRepository
{
    public DbDoctorRepository(AppDbContext context, IConverter<Doctor, DoctorDataModel> converter) 
        : base(context, converter)
    {
    }

    public async Task<Doctor?> GetDoctorByRoom(Guid roomId)
    {
        var doctorData = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.RoomId == roomId);
        return doctorData is null ? null : _converter.MapToEntity(doctorData);
    }

    public async Task<IReadOnlyCollection<Doctor>> GetDoctors()
    {
        var doctorsData = await _context.Doctors.AsNoTracking().ToListAsync();
        return doctorsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        var doctorData = _converter.MapToData(doctor);
        _context.Doctors.Update(doctorData);
        return Task.FromResult(doctor);
    }

    public async Task<Doctor> AddDoctor(Doctor doctor)
    {
        var doctorData = _converter.MapToData(doctor);
        await _context.Doctors.AddAsync(doctorData);
        return doctor;
    }

    public async Task<Doctor?> DeleteDoctor(Guid doctorId)
    {
        var doctorData = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctorData is null)
        {
            return null;
        }
        _context.Remove(doctorData);
        return _converter.MapToEntity(doctorData);
    }
}