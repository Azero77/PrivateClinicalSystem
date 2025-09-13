using ClinicApp.Domain.SecretaryAgg;

namespace ClinicApp.Domain.Repositories;

public interface ISecretaryRepository : IRepository<Secretary>
{
    Task<Secretary> AddSecretary(Secretary secretary);
    Task<Secretary> UpdateSecretary(Secretary secretary);
    Task<Secretary?> DeleteSecretary(Guid secretaryId);
    Task<IReadOnlyCollection<Secretary>> GetSecretaries();
}
