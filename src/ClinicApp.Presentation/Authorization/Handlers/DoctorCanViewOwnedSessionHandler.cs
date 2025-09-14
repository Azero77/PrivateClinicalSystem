using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class DoctorCanViewOwnedSessionHandler(IHttpContextAccessor accessor,IDoctorRepository repo) : CanViewBaseHandler<CanViewSessions,
    SessionQueryType>(accessor)
{
    public override UserRole ScopedRole => UserRole.Doctor;

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
        var doctor = await repo.GetDoctorByUsedId(userId);
        return doctor?.Id ?? Guid.Empty;
    }

    protected override bool IsOwned(SessionQueryType resource, Guid? ownerId, UserRole role)
    {
        return resource.DoctorId == ownerId;
    }
}
