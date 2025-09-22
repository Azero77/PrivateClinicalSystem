using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class CheckEmailVerifiedUserAuthenticationMiddleware
    : UserAuthenticationMiddleware
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public CheckEmailVerifiedUserAuthenticationMiddleware(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public override async Task<IActionResult> Handle(ApplicationUser user,PageModel pageModel,string returnUrl)
    {
        if (user.EmailConfirmed && _next is not null)
        { 
            return await _next.Handle(user,pageModel,returnUrl);
        }
        //generate an email confirmation token

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        //after the user has successfully confirmed their email they should be moved to complete registration
        var completeRegistrationurl = pageModel.Url.Page("/Account/CompleteRegistration/Index", new { returnUrl });

        var callbackUrl = pageModel.Url.Page(
            "/Account/Features/ConfirmEmail",
            pageHandler: null,
            values: new { userId, code, returnUrl = completeRegistrationurl },
            protocol: pageModel.Request.Scheme);
        if(user.Email is not null)
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

        return pageModel.RedirectToPage("/Account/Features/RegisterConfirmation");
    }
}
