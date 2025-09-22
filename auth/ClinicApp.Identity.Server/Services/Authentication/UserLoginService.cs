using AspNetCoreGeneratedDocument;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Authentication.Middlwares;
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
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager)
    {
        //login flow, login the user:1-check email verification 2-check completed profile
        LoginFlow = checkEmailVerifiedUserAuthenticationMiddleware
            .SetNext(checkRegistrationCompleteUserAuthenticationMiddleware);
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Handle(ApplicationUser user, PageModel pageModel,
        string returnUrl)
    {
        return await LoginFlow.Handle(user, pageModel, returnUrl);
    }

    public Task<ApplicationUser?> FindByNameAsync(string userName)
    {
        return _userManager.FindByNameAsync(userName);
    }
}
