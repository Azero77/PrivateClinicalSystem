
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Presentation.Helpers;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class CanViewOwnSessionHandler : AuthorizationHandler<CanView>
{
    private static UserRole[] _supportedRoles = { UserRole.Patient, UserRole.Doctor };
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;

    public CanViewOwnSessionHandler(IHttpContextAccessor httpContextAccessor, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanView requirement)
    {
        var user = context.User;
        var role = user.GetRole();

        if (requirement.allowedRoles.Contains(role))
        {
            context.Succeed(requirement);
            return;
        }

        if (!_supportedRoles.Contains(role))
        {
            return;
        }

        if (context.Resource is IMiddlewareContext resolverContext) // GraphQL
        {
            await HandleGraphQLRequirementAsync(context, requirement, resolverContext);
        }
        else // REST
        {
            await HandleRestRequirementAsync(user,role,context, requirement);
        }
    }

    private async Task HandleRestRequirementAsync(ClaimsPrincipal user,UserRole role,AuthorizationHandlerContext context, CanView requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return;

        if (!Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            context.Fail();
            return;
        }

        var ownedEntityId = await GetOwnedEntityIdAsync(userId, role);
        if (ownedEntityId is null)
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

    private async Task HandleGraphQLRequirementAsync(AuthorizationHandlerContext context, CanView requirement, IResolverContext resolverContext)
    {
        var user = context.User;
        var role = user.GetRole();

        var domainId = await GetOwnedEntityIdAsync(user, role);
        if (domainId == Guid.Empty)
            return;

        if (resolverContext is IMiddlewareContext ctx)
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
    }

    private async Task<Guid?> GetOwnedEntityIdAsync(Guid userId, UserRole role)
    {
        if (role == UserRole.Doctor)
        {
            return (await _doctorRepository.GetDoctorByUsedId(userId))?.Id;
        }
        if (role == UserRole.Patient)
        {
            return (await _patientRepository.GetPatientByUserId(userId))?.Id;
        }
        return null;
    }
    
    private async Task<Guid> GetOwnedEntityIdAsync(ClaimsPrincipal user, UserRole role)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Guid.Empty;

        var id = await GetOwnedEntityIdAsync(Guid.Parse(userId), role);
        return id ?? Guid.Empty;
    }

    private bool IsOwned(SessionQueryType session, Guid? ownerId, UserRole role)
    {
        if (ownerId is null) return false;

        if (role == UserRole.Doctor)
        {
            return session.DoctorId == ownerId;
        }
        if (role == UserRole.Patient)
        {
            return session.PatientId == ownerId;
        }
        return false;
    }
}
