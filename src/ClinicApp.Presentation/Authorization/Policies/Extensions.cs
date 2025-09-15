
using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Authorization.Handlers;
using ClinicApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Policies;

public static class PolicyExtensions
{
    public static void AddPolicies(this AuthorizationOptions opts)
    {
        #region sessionPolicies
        opts.AddPolicy(PoliciesConstants.CanViewOwnSessionsPolicy, builder => builder.AddRequirements(new CanViewSessions([UserRole.Admin,UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanViewAllSessionsPolicy,builder => builder.RequireRole(UserRole.Admin.ToString(),UserRole.Secretary.ToString()));

        //this policy is made upon CanViewOwnSessionsPolicy
        opts.AddPolicy(PoliciesConstants.CanViewSessionStatus,builder => 
        builder.AddRequirements(new CanViewSessions([UserRole.Admin,UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanViewSessionDetails, builder =>
        builder.RequireRole(UserRole.Doctor.ToString())
        .AddRequirements(new CanViewSessions([])));//no one can see the session details except the doctor (not even sec or admin);


        //this policy is made upon CanViewOwnSessionsPolicy
        opts.AddPolicy(PoliciesConstants.CanViewSessionHistory, builder =>
        builder.AddRequirements(new CanViewSessions([UserRole.Admin, UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanEditSessionDetails,
            builder => builder.AddRequirements(new CanViewSessions([UserRole.Admin,UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanDeleteSession,
            builder => builder.AddRequirements(new CanViewSessions([UserRole.Secretary])));

        #endregion
        #region roomPolicies
        opts.AddPolicy(PoliciesConstants.CanManageRooms,builder => builder.RequireRole(UserRole.Admin.ToString()));
        opts.AddPolicy(PoliciesConstants.CanViewRooms,builder => builder.RequireRole(UserRole.Admin.ToString()));
        #endregion
        #region doctor
        opts.AddPolicy(PoliciesConstants.CanViewDoctorsInfo,builder => builder.RequireAssertion(_ => true)); //all users
        opts.AddPolicy(PoliciesConstants.CanViewDoctorWorkingTime, builder => builder.AddRequirements(new CanViewDoctorWorkingTime([UserRole.Admin, UserRole.Secretary])));

        opts.AddPolicy(PoliciesConstants.CanViewDoctorTimesOff, builder => builder.AddRequirements(new CanViewDoctorTimesOff([UserRole.Admin])));
        #endregion
        #region adminPolicies
        opts.AddPolicy(PoliciesConstants.CanManageUsers,builder => builder.RequireRole(UserRole.Admin.ToString()));
        #endregion

        #region patientPolicies
        opts.AddPolicy(PoliciesConstants.CanViewPatient, builder => builder.RequireRole(UserRole.Admin.ToString(),UserRole.Doctor.ToString()));
        #endregion
    }

    public static IServiceCollection AddPolicyServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, CanViewDoctorTimesOffHandler>();
        services.AddScoped<IAuthorizationHandler, CanViewDoctorWorkingTimeHandler>();
        services.AddScoped<IAuthorizationHandler, DoctorCanViewOwnedSessionHandler>();
        services.AddScoped<IAuthorizationHandler, PatientCanViewOwnedSessionHandler>();
        return services;
    }
}
