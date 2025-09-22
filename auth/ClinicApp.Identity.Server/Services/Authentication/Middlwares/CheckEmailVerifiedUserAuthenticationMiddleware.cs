using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class CheckEmailVerifiedUserAuthenticationMiddleware
    : UserAuthenticationMiddleware
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CheckEmailVerifiedUserAuthenticationMiddleware(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string password,
        string returnUrl, 
        List<Claim>? additionalClaim = null)
    {
        if (user.EmailConfirmed && _next is not null)
        {
            return await _next.Handle(user, password, returnUrl,additionalClaim);
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var completeRegistrationUrl = _linkGenerator.GetPathByPage(
            page: "/Account/CompleteRegistration/Index",
            values: new { returnUrl }
        );

        var callbackUrl = _linkGenerator.GetUriByPage(
            httpContext: _httpContextAccessor.HttpContext!,
            page: "/Account/Features/ConfirmEmail",
            values: new { userId = user.Id, code, returnUrl = completeRegistrationUrl }
        );

        if (user.Email is not null)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        }
        return new LoginResult(LoginFlowStatus.RequireEmailConfirmation,
            "/Account/Features/RegisterConfirmation");
    }
}
