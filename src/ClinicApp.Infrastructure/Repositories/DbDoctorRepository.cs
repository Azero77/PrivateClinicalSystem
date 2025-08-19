﻿using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.DataQueryHelpers;
using ClinicApp.Infrastructure.Persistance;
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
        Doctor? doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor is not null)
        {
            _context.Remove(doctor);
            await _context.SaveChangesAsync();
        }

        return doctor;
    }

    public Task<Doctor?> GetByIdAsync(Guid id)
    {
        return _context.Doctors.AsNoTracking().SingleOrDefaultAsync(d => d.Id == id);
    }
    public Task<Doctor?> GetDoctorByRoom(Guid roomId)
    {
        return _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.RoomId == roomId);
    }

    public async Task<IReadOnlyCollection<Doctor>> GetDoctors()
    {
        var list = await _context.Doctors.AsNoTracking().ToListAsync();
        return list.AsReadOnly();
    }

    public async Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }
    public async Task<Doctor?> AddDoctor(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
        return doctor;
    }
}
