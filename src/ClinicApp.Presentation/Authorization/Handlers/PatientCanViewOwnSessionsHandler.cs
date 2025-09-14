using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;

public class PatientCanViewOwnSessionsHandler(IHttpContextAccessor httpContextAccessor,
                                             IPatientRepository repo) : BaseOwnSessionHandler(httpContextAccessor)
{
    protected override UserRole SupportedRole => UserRole.Patient;

    protected override async Task<Guid?> GetOwnedEntityIdAsync(Guid userId)
    {
        return (await repo.GetPatientByUserId(userId))?.Id;
    }
}
