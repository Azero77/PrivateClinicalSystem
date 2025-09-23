using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class CheckRegistrationCompleteUserAuthenticationMiddleware : UserAuthenticationMiddleware
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _linkGenerator;

    public CheckRegistrationCompleteUserAuthenticationMiddleware(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor accessor,
        LinkGenerator linkGenerator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _accessor = accessor;
        _linkGenerator = linkGenerator;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string password,
        string returnUrl, List<Claim>? additionalClaims = null)
    {
        if (_accessor.HttpContext is null)
            return new LoginResult(LoginFlowStatus.AccessDenied, returnUrl);

        // Create a principal from DB claims
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        // Add any transient session claims (like idp, id_token)
        if (additionalClaims != null && additionalClaims.Any())
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            foreach (var claim in additionalClaims)
            {
                identity.AddClaim(claim);
            }
        }

        // Check if profile is complete
        var profileClaim = principal.FindFirst(ServerConstants.CompleteProfileClaimKey);
        var completeProfileUrl = _linkGenerator.GetPathByPage(
            page: "/Account/CompleteRegistration/Index",
            values: new { returnUrl }
        );

        if (profileClaim is null)
        {
            // Persist incomplete-profile claim to DB
            await _userManager.AddClaimAsync(user,
                new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));

            // Sign in with transient claims preserved
            await _accessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties { IsPersistent = true });

            return new LoginResult(LoginFlowStatus.RequireProfileCompletion, completeProfileUrl);
        }

        if (profileClaim.Value == ServerConstants.UnCompletedProfileClaimValue)
        {
            // User already has incomplete-profile claim in DB
            await _accessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties { IsPersistent = true });

            return new LoginResult(LoginFlowStatus.RequireProfileCompletion, completeProfileUrl);
        }

        // User has completed registration — include transient claims for this session
        await _accessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties { IsPersistent = true });

        // Continue to next middleware / flow
        return await _next!.Handle(user, password, returnUrl, additionalClaims);
    }
}
