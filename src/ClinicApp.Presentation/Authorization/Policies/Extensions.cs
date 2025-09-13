
using ClinicApp.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Policies;

public static class PolicyExtensions
{
    public static void AddPolicies(this AuthorizationOptions opts)
    {
        opts.AddPolicy(PoliciesConstants.CanViewOwnSessionsPolicy, builder => builder.AddRequirements(new CanViewOwnSessions([UserRole.Admin,UserRole.Secretary]))
        .RequireAuthenticatedUser()
        .RequireClaim(ClaimTypes.NameIdentifier));

        opts.AddPolicy(PoliciesConstants.CanViewAllSessionsPolicy,builder => builder.RequireRole(UserRole.Admin.ToString(),UserRole.Secretary.ToString()));

        //this policy is made upon CanViewOwnSessionsPolicy
        opts.AddPolicy(PoliciesConstants.CanViewSessionStatus,builder => 
        builder.AddRequirements(new CanViewOwnSessions([UserRole.Admin,UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanViewSessionDetails, builder =>
        builder.RequireRole(UserRole.Doctor.ToString())
        .AddRequirements(new CanViewOwnSessions([])));//no one can see the session details except the doctor (not even sec or admin);


        //this policy is made upon CanViewOwnSessionsPolicy
        opts.AddPolicy(PoliciesConstants.CanViewSessionHistory, builder =>
        builder.AddRequirements(new CanViewOwnSessions([UserRole.Admin, UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanEditSessionDetails,
            builder => builder.AddRequirements(new CanViewOwnSessions([UserRole.Admin,UserRole.Secretary]))
            .RequireRole(UserRole.Admin.ToString(),UserRole.Secretary.ToString(),UserRole.Doctor.ToString()));

        opts.AddPolicy(PoliciesConstants.CanDeleteSession,
            builder => builder.AddRequirements(new CanViewOwnSessions([UserRole.Secretary]))
            .RequireRole(UserRole.Doctor.ToString(),UserRole.Secretary.ToString()));
    }
}
