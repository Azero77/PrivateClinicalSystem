using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ClinicApp.Presentation.Authorization.Handlers;

public abstract class CanModifySession<TRequirement, TRequest> : AuthorizationHandler<TRequirement, TRequest>
    where TRequirement : CanModifySessionRequirement, new()
{
    private static UserRole[] AllowedRoles = { UserRole.Admin, UserRole.Secretary };
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, TRequest resource)
    {
        var userRole = context.User.GetRole();
        if (AllowedRoles.Contains(userRole))
        {
            context.Succeed(requirement);return;
        }
        else if (userRole != UserRole.Doctor) 
        {
            context.Fail();return;
        }

        if (Guid.TryParse(context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out Guid userId))
        {
            context.Fail();
            return;
        }
        if (await Authorize(context, requirement, resource, userId))
            context.Succeed(requirement);
        else
        {
            context.Fail();
        }
    }
    protected abstract Task<bool> Authorize(AuthorizationHandlerContext context, TRequirement requirement, TRequest resource, Guid userId);
}

