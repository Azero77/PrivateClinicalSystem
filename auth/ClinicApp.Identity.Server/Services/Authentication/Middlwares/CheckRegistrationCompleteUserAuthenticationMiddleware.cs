using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class CheckRegistrationCompleteUserAuthenticationMiddleware
    : UserAuthenticationMiddleware
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _linkGenerator;

    public CheckRegistrationCompleteUserAuthenticationMiddleware(SignInManager<ApplicationUser> signInManager,
                                                                 UserManager<ApplicationUser> userManager,
                                                                 IHttpContextAccessor accessor,
                                                                 LinkGenerator linkGenerator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _accessor = accessor;
        _linkGenerator = linkGenerator;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user, string returnUrl)
    {
        if (_accessor.HttpContext is null)
            return new LoginResult(LoginFlowStatus.AccessDenied,returnUrl);
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        //check if {profile_stage:completed} claim is found or not
        await _accessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        var claim = principal.FindFirst(ServerConstants.CompleteProfileClaimKey);
        var completeRegistrationUrl = _linkGenerator.GetPathByPage(
           page: "/Account/CompleteRegistration/Index",
           values: new { returnUrl }
       );
        if (claim is null)
        {
            await _userManager.AddClaimAsync(user, new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));
            await _signInManager.RefreshSignInAsync(user);
            return new LoginResult(LoginFlowStatus.RequireProfileCompletion,completeRegistrationUrl);
        }
        else if (claim!.Value == ServerConstants.UnCompletedProfileClaimValue)
        {
            return new LoginResult(LoginFlowStatus.RequireProfileCompletion,completeRegistrationUrl);
        }

        //the user has completed registration we will return them to the page
        //will go to /connect/authorize endpoint to generate the jwt
        return await _next!.Handle(user, returnUrl);
    }
}