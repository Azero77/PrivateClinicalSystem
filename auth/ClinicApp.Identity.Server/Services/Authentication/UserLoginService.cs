using AspNetCoreGeneratedDocument;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Authentication.Middlwares;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicApp.Identity.Server.Services.Authentication;

public class UserLoginService
{
    public UserAuthenticationMiddleware LoginFlow { get; }
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public UserLoginService(CheckEmailVerifiedUserAuthenticationMiddleware checkEmailVerifiedUserAuthenticationMiddleware,
                            CheckRegistrationCompleteUserAuthenticationMiddleware checkRegistrationCompleteUserAuthenticationMiddleware,
                            CheckAuthorizationContextClientRequestMiddleware checkAuthorizationContextClientRequestMiddleware,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager)
    {
        //login flow, login the user:1-check email verification 2-check completed profile
        LoginFlow = checkEmailVerifiedUserAuthenticationMiddleware
            .SetNext(checkRegistrationCompleteUserAuthenticationMiddleware)
            .SetNext(checkAuthorizationContextClientRequestMiddleware);
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string returnUrl)
    {
        return await LoginFlow.Handle(user, returnUrl);
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
    RequireEmailConfirmation,
    RequireProfileCompletion,
    AccessDenied
}