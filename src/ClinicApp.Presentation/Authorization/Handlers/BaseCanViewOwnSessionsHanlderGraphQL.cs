
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Helpers;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public abstract class BaseCanViewOwnSessionsHanlderGraphQL : AuthorizationHandler<CanView, IResolverContext>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CanView requirement,
        IResolverContext resource)
    {
        var user = context.User;
        var role = user.GetRole();
        if (requirement.allowedRoles.Contains(role))
        {
            context.Succeed(requirement);return;
        }
        if (role != SupportedRole)
            return;
        var domainId = await GetDomainId(user);
        if (domainId == Guid.Empty)
            return;
        if (resource is IMiddlewareContext ctx)
        {
            if (ctx.Result is SessionQueryType session)
            {
                if (IsOwned(session, domainId))
                {
                    context.Succeed(requirement); return;
                }
                context.Fail();
            }
            else if (ctx.Result is IEnumerable<SessionQueryType> sessions)
            {
                if (sessions.All(s => IsOwned(s, domainId)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

        }
    }


    protected abstract UserRole SupportedRole { get; }

    /// <summary>
    /// Maps from the authenticated userId (Guid) to the domain entity’s Id (doctorId, patientId, etc.)
    /// </summary>
    protected abstract Func<ClaimsPrincipal, Task<Guid>> GetDomainId { get; }
    protected abstract Func<SessionQueryType, Guid, bool> IsOwned { get; } //will compare a session property to the user domain id
}
