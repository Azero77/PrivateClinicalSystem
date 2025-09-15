using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class DoctorCanViewOwnedSessionHandler(IHttpContextAccessor accessor,IDoctorRepository repo) : CanViewBaseHandler<CanViewSessions,
    SessionQueryType>(accessor)
{
    public override UserRole ScopedRole => UserRole.Doctor;
    private static IEnumerable<SessionQueryType> ExtractSessions(object? result)
    {
        return result switch
        {
            SessionQueryType s => new[] { s },
            IEnumerable<SessionQueryType> ss => ss,
            Connection<SessionQueryType> c => c.Edges.Select(e => e.Node),
            _ => Enumerable.Empty<SessionQueryType>()
        };
    }
    protected override void CheckOwnedGraphQL(AuthorizationHandlerContext context, CanViewSessions requirement, UserRole role, Guid domainId, IMiddlewareContext ctx)
    {
        var sessions = ExtractSessions(ctx.Result);
        if (sessions.Any() && sessions.All(s => IsOwned(s, domainId, role)))
            context.Succeed(requirement);
        else
            context.Fail();
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
