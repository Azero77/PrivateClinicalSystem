using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class SignInUserAuthenticationMiddleware : UserAuthenticationMiddleware
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SignInUserAuthenticationMiddleware(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user, string password,string returnUrl, List<Claim>? additionalClaim = null)
    {
        SignInResult isSignedIn = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!isSignedIn.Succeeded)
        {
            return new LoginResult(LoginFlowStatus.PasswordDoNotMatch, returnUrl);
        }

        if (_next is null)
        {
            throw new ArgumentException("No Next middleware is added");
        }
        return await _next!.Handle(user, password,returnUrl,additionalClaim);
    }
}