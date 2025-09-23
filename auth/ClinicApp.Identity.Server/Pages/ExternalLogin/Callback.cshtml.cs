using System.Security.Claims;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services.Authentication;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuickStart3.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserLoginService _userLoginService;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        UserLoginService userLoginService)
    {

        _interaction = interaction;
        _logger = logger;
        _events = events;
        _userManager = userManager;
        _signInManager = signInManager;
        _userLoginService = userLoginService;
    }

    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true)
        {
            throw new InvalidOperationException($"External authentication error: {result.Failure}");
        }

        var externalUser = result.Principal ??
            throw new InvalidOperationException("External authentication produced a null Principal");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.ExternalClaims(externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new InvalidOperationException("Unknown userid");

        var username = externalUser.FindFirst(JwtClaimTypes.Name);
        var email = externalUser.FindFirst(JwtClaimTypes.Email);
        var provider = result.Properties.Items["scheme"] ?? throw new InvalidOperationException("Null scheme in authentication properties");
        var providerUserId = userIdClaim.Value;
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // find external user
        var user = await _userManager.FindByLoginAsync(provider, providerUserId);
        if (user == null)
        {
            Guid sub = Guid.NewGuid();
            ApplicationUser newUser = new ApplicationUser()
            {
                Id = sub,
                UserName = email?.Value ?? Guid.NewGuid().ToString(),
                Email = email?.Value,
                EmailConfirmed = true // here ofcourse the user is logged in
            };
            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                throw new ArgumentException("sorry,Something went wrong during login");
            }
            var loginInfo = new UserLoginInfo(provider, providerUserId, provider);
            var addLoginResult = await _userManager.AddLoginAsync(newUser, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                throw new ArgumentException("sorry,Something went wrong during login");
            }
            user = newUser;
        }

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
      var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";
        var loginResult = await _userLoginService.HandleExternal(user,returnUrl,additionalLocalClaims);
        // delete temporary cookie used during external authentication

        if (loginResult.IsError)
        {
            ModelState.AddModelError("", loginResult.FirstError.Description);
            return Page();
        }
        var value = loginResult.Value;
        if (value.status == LoginFlowStatus.LoginSucceed)
        {
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        }
        return value.ToActionResult(this);
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private static void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        ArgumentNullException.ThrowIfNull(externalResult.Principal, nameof(externalResult.Principal));

        // capture the idp used to login, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties?.Items["scheme"] ?? "unknown identity provider"));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties?.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}
