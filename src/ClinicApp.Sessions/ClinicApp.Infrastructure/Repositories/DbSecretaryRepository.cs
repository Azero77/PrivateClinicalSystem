using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SecretaryAgg;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbSecretaryRepository : Repository<Secretary, SecretaryDataModel>, ISecretaryRepository
{
    public DbSecretaryRepository(AppDbContext context, IConverter<Secretary, SecretaryDataModel> converter) : base(context, converter)
    {
    }

    public async Task<Secretary> AddSecretary(Secretary secretary)
    {
        var secretaryData = _converter.MapToData(secretary);
        await _context.Secretaries.AddAsync(secretaryData);
        return secretary;
    }

    public async Task<Secretary?> DeleteSecretary(Guid secretaryId)
    {
        var secretaryData = await _context.Secretaries.FirstOrDefaultAsync(s => s.Id == secretaryId);
        if (secretaryData is null)
        {
            return null;
        }
        _context.Secretaries.Remove(secretaryData);
        return _converter.MapToEntity(secretaryData);
    }

    public async Task<IReadOnlyCollection<Secretary>> GetSecretaries()
    {
        var secretariesData = await _context.Secretaries.AsNoTracking().ToListAsync();
        return secretariesData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Secretary> UpdateSecretary(Secretary secretary)
    {
        var secretaryData = _converter.MapToData(secretary);
        _context.Secretaries.Update(secretaryData);
        return Task.FromResult(secretary);
    }
}
