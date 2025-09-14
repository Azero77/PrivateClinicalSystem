
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class DoctorCanViewOwnSessionHandlerGraphQL : BaseCanViewOwnSessionsHanlderGraphQL
{
    protected override UserRole SupportedRole => UserRole.Doctor;

    protected override Func<ClaimsPrincipal, Task<Guid>> GetDomainId => async user =>
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Guid.Empty;
        var doctor = await _repo.GetDoctorByUsedId(Guid.Parse(userId));
        return doctor?.Id ?? Guid.Empty;
    };

    protected override Func<SessionQueryType, Guid, bool> IsOwned => (session,doctorId) => session.DoctorId == doctorId;

    private readonly IDoctorRepository _repo;

    public DoctorCanViewOwnSessionHandlerGraphQL(IDoctorRepository repo)
    {
        _repo = repo;
    }

}
