using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Helpers;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public abstract class CanViewBaseHandler<TCanView,TResource> : AuthorizationHandler<TCanView>
    where TCanView : CanView
    where TResource : class
{
    /// <summary>
    /// The resource role 
    /// </summary>
    public abstract UserRole ScopedRole { get; }
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected CanViewBaseHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TCanView requirement)
    {

        var user = context.User;
        var role = user.GetRole();

        if (requirement.allowedRoles.Contains(role))
        {
            context.Succeed(requirement);
            return;
        }

        if (ScopedRole != (role))
        {
            return;
        }

        if (!Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            context.Fail();
            return;
        }



        if (context.Resource is IMiddlewareContext resolverContext) // GraphQL
        {
            await HandleGraphQLRequirementAsync(userId,role,context, requirement, resolverContext);
        }
        else // REST
        {
            await HandleRestRequirementAsync(userId, role, context, requirement);
        }
    }

    protected async Task HandleGraphQLRequirementAsync(Guid userId, UserRole role,AuthorizationHandlerContext context, TCanView requirement, IMiddlewareContext ctx)
    {
        var domainId = await GetOwnedEntityIdAsync(userId, role);
        if (domainId == Guid.Empty)
            return;
        CheckOwnedGraphQL(context, requirement, role, domainId, ctx);
    }

    private async Task HandleRestRequirementAsync(Guid userId, UserRole role, AuthorizationHandlerContext context, TCanView requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return;
        var ownedEntityId = await GetOwnedEntityIdAsync(userId, role);
        if (ownedEntityId == Guid.Empty)
        {
            context.Fail();
            return;
        }

        var routeIds = httpContext.GetRouteData().Values
            .Where(kvp => kvp.Key.Contains("id", StringComparison.InvariantCultureIgnoreCase))
            .Select(kvp => Guid.TryParse(kvp.Value?.ToString(), out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

        if (!routeIds.Any(id => id == ownedEntityId))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    protected abstract void CheckOwnedGraphQL(AuthorizationHandlerContext context, TCanView requirement, UserRole role, Guid domainId, IMiddlewareContext ctx);


    protected abstract bool IsOwned(TResource resource, Guid? ownerId, UserRole role);
    protected abstract Task<Guid> GetOwnedEntityIdAsync(Guid userId, UserRole role);
}
