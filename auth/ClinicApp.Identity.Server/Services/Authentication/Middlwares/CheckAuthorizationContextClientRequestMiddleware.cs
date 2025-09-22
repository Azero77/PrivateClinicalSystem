using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Duende.IdentityServer.Services;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickStart3.Pages;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;
public class CheckAuthorizationContextClientRequestMiddleware : UserAuthenticationMiddleware
{
    private readonly IIdentityServerInteractionService _interaction;

    public CheckAuthorizationContextClientRequestMiddleware(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public override async Task<ErrorOr<LoginResult>> Handle(ApplicationUser user,
        string password,
        string returnUrl, 
        List<Claim>? additionalClaim = null)
    {
        if (string.IsNullOrEmpty(returnUrl))
            throw new ArgumentNullException(nameof(returnUrl), "Return URL can't be null");

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        if (context != null)
        {
            // Authorization context exists → user can log in
            return new LoginResult(LoginFlowStatus.LoginSucceed, returnUrl);
        }

        // Authorization context is null → fallback: check if local URL
        if (Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            return new LoginResult(LoginFlowStatus.LoginSucceed, returnUrl);
        }

        if (string.IsNullOrEmpty(returnUrl))
        {
            return new LoginResult(LoginFlowStatus.LoginSucceed, "~/");
        }

        // Invalid/malicious URL
        throw new ArgumentException("Invalid return URL", nameof(returnUrl));
    }
}
