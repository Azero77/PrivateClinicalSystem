using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbPatientRepository : Repository<Patient, PatientDataModel>, IPatientRepository
{
    public DbPatientRepository(AppDbContext context, IConverter<Patient, PatientDataModel> converter) : base(context, converter)
    {
    }

    public async Task<Patient> AddPatient(Patient patient)
    {
        var patientData = _converter.MapToData(patient);
        await _context.Patients.AddAsync(patientData);
        return patient;
    }

    public async Task<Patient?> DeletePatient(Guid patientId)
    {
        var patientData = await _context.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
        if (patientData is null)
        {
            return null;
        }
        _context.Patients.Remove(patientData);
        return _converter.MapToEntity(patientData);
    }

    public async Task<Patient?> GetPatientByUserId(Guid userId)
    {
        var dm = await _context.Patients.SingleOrDefaultAsync(p => p.UserId == userId);
        return dm is null ? null : _converter.MapToEntity(dm);
    }

    public async Task<IReadOnlyCollection<Patient>> GetPatients()
    {
        var patientsData = await _context.Patients.AsNoTracking().ToListAsync();
        return patientsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Patient> UpdatePatient(Patient patient)
    {
        var patientData = _converter.MapToData(patient);
        _context.Patients.Update(patientData);
        return Task.FromResult(patient);
    }
}