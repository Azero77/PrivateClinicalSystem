using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class PatientCanViewOwnedSessionHandler(IHttpContextAccessor accessor, IPatientRepository repo) : CanViewBaseHandler<CanViewSessions,
    SessionQueryType>(accessor)
{
    public override UserRole ScopedRole => throw new NotImplementedException();

    protected override void CheckOwnedGraphQL(AuthorizationHandlerContext context, CanViewSessions requirement, UserRole role, Guid domainId, IMiddlewareContext ctx)
    {

        if (ctx.Result is SessionQueryType session)
        {
            if (IsOwned(session, domainId, role))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
        else if (ctx.Result is IEnumerable<SessionQueryType> sessions)
        {
            if (sessions.All(s => IsOwned(s, domainId, role)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

    protected override async Task<Guid> GetOwnedEntityIdAsync(Guid userId, UserRole role)
    {

        var patient = await repo.GetPatientByUserId(userId);
        return patient?.Id ?? Guid.Empty;
    }

    protected override bool IsOwned(SessionQueryType resource, Guid? ownerId, UserRole role)
    {
        return resource.PatientId == ownerId;
    }
}