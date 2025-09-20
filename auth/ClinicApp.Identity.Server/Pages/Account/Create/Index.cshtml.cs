using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuickStart3.Pages.Login;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuickStart3.Pages.Create;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;

    public ViewModel View { get; set; } = null!;
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public Index(
        IIdentityServerInteractionService interaction,
        UserManager<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        IIdentityProviderStore identityProviderStore,
        IAuthenticationSchemeProvider schemeProvider)
    {
        _userManager = userStore;
        _interaction = interaction;
        _signInManager = signInManager;
        _identityProviderStore = identityProviderStore;
        _schemeProvider = schemeProvider;
    }

    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        await BuildModelAsync(returnUrl);
        if (View.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        // the user clicked the "cancel" button
        if (Input.Button != "create")
        {
            if (context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                return Redirect(Input.ReturnUrl ?? "~/");
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }
        }

        

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(Input?.Username ?? string.Empty);
            if (user is not null)
            {
                ModelState.AddModelError("Input.Username", $"{user.UserName} is already taken");
                await BuildModelAsync(Input!.ReturnUrl);
                return Page();
            }
            Guid sub = Guid.NewGuid();
            ApplicationUser newUser = new ApplicationUser()
            {
                Id = sub,
                UserName = Input!.Username,
                Email = Input!.Email,
            };
            var result = await _userManager.CreateAsync(newUser,Input.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                await BuildModelAsync(Input!.ReturnUrl);
                return Page();
            }
            await _userManager.AddClaimAsync(newUser,
    new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));


            var principal = await _signInManager.CreateUserPrincipalAsync(newUser);
            var identity = (ClaimsIdentity)principal.Identity!;
            identity.AddClaim(new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));

            // issue authentication cookie with subject ID and username
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,principal);
            if (!Url.IsLocalUrl(Input.ReturnUrl))
            {
                throw new ArgumentException("Invalid Return Url");
            }
            string stage2Url = Url.Page("/Account/CompleteRegistration/Index",new { returnUrl = Input.ReturnUrl})!;
            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(Input.ReturnUrl);
                }

                // we can trust Input.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(stage2Url);
            }
            return Redirect(stage2Url); //local page
        }
        await BuildModelAsync(Input!.ReturnUrl);
        return Page();
    }

    private async Task BuildModelAsync(string? returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl ?? "/"
        };

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null)
        {
            var scheme = await _schemeProvider.GetSchemeAsync(context.IdP);
            if (scheme != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                View = new ViewModel
                {
                    EnableLocalLogin = local,
                };

                Input.Username = context?.LoginHint ?? string.Empty;

                if (!local)
                {
                    View.ExternalProviders = [new ViewModel.ExternalProvider(authenticationScheme: context.IdP, displayName: scheme.DisplayName)];
                }
            }

            return;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            (
                authenticationScheme: x.Name,
                displayName: x.DisplayName ?? x.Name
            )).ToList();

        var dynamicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            (
                authenticationScheme: x.Scheme,
                displayName: x.DisplayName ?? x.Scheme
            ));
        providers.AddRange(dynamicSchemes);


        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count != 0)
            {
                providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }
}
