using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;

public class DoctorCanViewOwnSessionsHandler(IHttpContextAccessor httpContextAccessor,
                                             IDoctorRepository repo) : BaseOwnSessionHandler(httpContextAccessor)
{
    protected override UserRole SupportedRole => UserRole.Doctor;

    protected override async Task<Guid?> GetOwnedEntityIdAsync(Guid userId)
    {
        return (await repo.GetDoctorByUsedId(userId))?.Id;
    }
}
