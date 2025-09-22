using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
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

    public CheckRegistrationCompleteUserAuthenticationMiddleware(SignInManager<ApplicationUser> signInManager,
                                                                 UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public override async Task<IActionResult> Handle(ApplicationUser user, PageModel pageModel, string returnUrl)
    {
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        //check if {profile_stage:completed} claim is found or not
        await pageModel.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        var claim = principal.FindFirst(ServerConstants.CompleteProfileClaimKey);
        var completeRegistrationUrl = pageModel.Url.Page("/Account/CompleteRegistration/Index", new { returnUrl});

        if (claim is null)
        {
            await _userManager.AddClaimAsync(user, new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));
            await _signInManager.RefreshSignInAsync(user);
            return pageModel.LocalRedirect(completeRegistrationUrl!);
        }
        else if (claim!.Value == ServerConstants.UnCompletedProfileClaimValue)
        {
            return pageModel.LocalRedirect(completeRegistrationUrl!);
        }

        //the user has completed registration we will return them to the page
        //will go to /connect/authorize endpoint to generate the jwt
        if (_next is not null)
            return await _next.Handle(user, pageModel, returnUrl);
        return pageModel.LocalRedirect(returnUrl);
    }
}