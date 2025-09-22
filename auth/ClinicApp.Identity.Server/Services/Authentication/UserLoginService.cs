using AspNetCoreGeneratedDocument;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Authentication.Middlwares;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ClinicApp.Identity.Server.Services.Authentication;

public class UserLoginService
{
    public UserAuthenticationMiddleware LoginFlow { get; }
    public UserAuthenticationMiddleware ExternalLoginFlow { get; }
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public UserLoginService(
                            SignInUserAuthenticationMiddleware signInUserAuthenticationMiddleware,
                            CheckEmailVerifiedUserAuthenticationMiddleware checkEmailVerifiedUserAuthenticationMiddleware,
                            CheckRegistrationCompleteUserAuthenticationMiddleware checkRegistrationCompleteUserAuthenticationMiddleware,
                            CheckAuthorizationContextClientRequestMiddleware checkAuthorizationContextClientRequestMiddleware,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager)
    {
        //login flow, login the user:1-check email verification 2-check completed profile
        LoginFlow = signInUserAuthenticationMiddleware;
        LoginFlow
            .SetNext(checkEmailVerifiedUserAuthenticationMiddleware)
            .SetNext(checkRegistrationCompleteUserAuthenticationMiddleware)
            .SetNext(checkAuthorizationContextClientRequestMiddleware);

        ExternalLoginFlow = checkEmailVerifiedUserAuthenticationMiddleware;
        ExternalLoginFlow
            .SetNext(checkEmailVerifiedUserAuthenticationMiddleware)
            .SetNext(checkRegistrationCompleteUserAuthenticationMiddleware)
            .SetNext(checkAuthorizationContextClientRequestMiddleware);
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string password,
        string returnUrl,
        List<Claim>? additionalClaims = null)
    {
        return await LoginFlow.Handle(user, password,returnUrl, additionalClaims);
    }

    public async Task<ErrorOr<LoginResult>> HandleExternal(ApplicationUser user,
        string password,
        string returnUrl,
        List<Claim>? additionalClaims)
    {
        return await LoginFlow.Handle(user, password, returnUrl, additionalClaims);
    }

    public Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }
}
public record LoginResult(LoginFlowStatus status,string? returnUrl);
public enum LoginFlowStatus
{
    LoginSucceed,
    PasswordDoNotMatch,
    RequireEmailConfirmation,
    RequireProfileCompletion,
    AccessDenied
}

public class SignInUserAuthenticationMiddleware : UserAuthenticationMiddleware
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SignInUserAuthenticationMiddleware(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user, string password,string returnUrl, List<Claim>? additionalClaim = null)
    {
        SignInResult isSignedIn = await _signInManager.CheckPasswordSignInAsync(user, password, true);
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