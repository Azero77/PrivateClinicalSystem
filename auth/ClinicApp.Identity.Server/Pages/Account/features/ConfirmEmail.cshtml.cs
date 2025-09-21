#nullable disable

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Duende.IdentityServer.Services;
using System.Security.Claims;
using ClinicApp.Identity.Server.Constants;
using QuickStart3.Pages;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;

namespace ClinicApp.Identity.Server.Pages.Account.Features;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    public ConfirmEmailModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
    }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl)
    {
        if (userId == null || code == null)
        {
            return RedirectToPage("/Index");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        
        if(result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.UnCompletedProfileClaimValue));

            // We need to issue a cookie so the user can navigate to the next step.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            string stage2Url = Url.Page("/Account/CompleteRegistration/Index", new { returnUrl });
            
            if (returnUrl != null)
            {
                var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        return this.LoadingPage(stage2Url);
                    }
                    return Redirect(stage2Url);
                }
            }

            return Redirect(stage2Url ?? "/");
        }

        StatusMessage = "Error confirming your email.";
        return Page();
    }
}