
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class PatientCanViewOwnSessionHandlerGraphQL : BaseCanViewOwnSessionsHanlderGraphQL
{
    protected override UserRole SupportedRole => UserRole.Patient;

    protected override Func<ClaimsPrincipal, Task<Guid>> GetDomainId => async user =>
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Guid.Empty;
        var patient = await _repo.GetPatientByUserId(Guid.Parse(userId));
        return patient?.Id ?? Guid.Empty;
    };

    protected override Func<SessionQueryType, Guid, bool> IsOwned => (session, patientId) => session.PatientId == patientId;

    private readonly IPatientRepository _repo;

    public PatientCanViewOwnSessionHandlerGraphQL(IPatientRepository repo)
    {
        _repo = repo;
    }

}