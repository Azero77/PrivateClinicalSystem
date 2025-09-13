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
    public async Task<Patient?> GetPatientByUserId(Guid userId)
    {
        var dm = await _context.Patients.SingleOrDefaultAsync(p => p.UserId == userId);
        return dm is null ? null : _converter.MapToEntity(dm);
    }
}