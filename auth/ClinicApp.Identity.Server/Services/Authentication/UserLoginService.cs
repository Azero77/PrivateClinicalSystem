using AspNetCoreGeneratedDocument;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Authentication.Middlwares;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Services.Authentication;

public class UserLoginService
{
    public UserAuthenticationMiddleware LoginFlow { get; }
    public UserAuthenticationMiddleware ExternalLoginFlow { get; }
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IDomainUserRegister _register;
    public UserLoginService(
                            SignInUserAuthenticationMiddleware signInUserAuthenticationMiddleware,
                            CheckEmailVerifiedUserAuthenticationMiddleware checkEmailVerifiedUserAuthenticationMiddleware,
                            CheckRegistrationCompleteUserAuthenticationMiddleware checkRegistrationCompleteUserAuthenticationMiddleware,
                            CheckAuthorizationContextClientRequestMiddleware checkAuthorizationContextClientRequestMiddleware,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IDomainUserRegister registerer)
    {
        //login flow, login the user:1-check email verification 2-check completed profile
        LoginFlow = signInUserAuthenticationMiddleware;
        LoginFlow
            .SetNext(checkEmailVerifiedUserAuthenticationMiddleware)
            .SetNext(checkRegistrationCompleteUserAuthenticationMiddleware)
            .SetNext(checkAuthorizationContextClientRequestMiddleware);

        ExternalLoginFlow = checkRegistrationCompleteUserAuthenticationMiddleware;
        ExternalLoginFlow
            .SetNext(checkAuthorizationContextClientRequestMiddleware);
        _userManager = userManager;
        _signInManager = signInManager;
        _register = registerer;
    }

    public async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string password,
        string returnUrl,
        List<Claim>? additionalClaims = null)
    {
        return await LoginFlow.Handle(user, password,returnUrl, additionalClaims);
    }

    public async Task<ErrorOr<LoginResult>> HandleExternal(ApplicationUser user,
        string returnUrl,
        List<Claim>? additionalClaims)
    {
        return await ExternalLoginFlow.Handle(user, string.Empty, returnUrl, additionalClaims);
    }

    public Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }

    public async Task<ErrorOr<LoginResult>> CompleteRegistrationFor(ApplicationUser user,
        string returnUrl,
        DomainUserRegisterContext context)
    {
        var selectedRole = context.SelectedRole;

        await _register.Modify(user!, new DomainUserRegisterContext() { SelectedRole = selectedRole });
        await _signInManager.RefreshSignInAsync(user);
        //publish an integration event for created user with role
        return new LoginResult(LoginFlowStatus.LoginSucceed,returnUrl);
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
