using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public abstract class BaseOwnSessionHandler : AuthorizationHandler<CanView>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseOwnSessionHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanView requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return;

        var role = httpContext.User.GetRole();
        // only handle if current role matches the one this handler supports
        if (requirement.allowedRoles.Contains(role))
        {
            context.Succeed(requirement);
            return;
        }
        if (role != SupportedRole)
            return;
        if (!Guid.TryParse(
                httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out Guid userId))
        {
            context.Fail();
            return;
        }

        // extract route ids when using rest api
        var routeIds = httpContext.GetRouteData().Values
            .Where(kvp => kvp.Key.Contains("id", StringComparison.InvariantCultureIgnoreCase))
            .Select(kvp => Guid.TryParse(kvp.Value?.ToString(), out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

        //extract 

        var ownedEntityId = await GetOwnedEntityIdAsync(userId);
        if (ownedEntityId is null || !routeIds.Any(id => id == ownedEntityId))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    protected abstract UserRole SupportedRole { get; }

    /// <summary>
    /// Maps from the authenticated userId (Guid) to the domain entity’s Id (doctorId, patientId, etc.)
    /// </summary>
    protected abstract Task<Guid?> GetOwnedEntityIdAsync(Guid userId);
}
