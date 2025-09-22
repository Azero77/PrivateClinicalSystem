using ClinicApp.Domain.Common;
using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Pages.Account.CompleteRegistration
{
    //[Authorize(Policy = ServerConstants.UnCompletedProfilePolicy)]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDomainUserRegister _register;
        public IEnumerable<SelectListItem> AllowedRoles => Enum.GetValues<UserRole>()
            .Where(DomainUserRegister.AllowedRoles.Contains)
            .Select(r => new SelectListItem()
            {
                Value = r.ToString(),
                Text = r.ToString()
            });

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IDomainUserRegister register)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _register = register;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;
        public void OnGet()
        {
        }
        public class InputModel
        {
            public string? ReturnUrl { get; set; }
            [Required]
            public UserRole Role { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            UserRole selectedRole = Input.Role;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                ModelState.AddModelError("", "Unknown Error Happend,Try Again later");
                return Page();
            }
            await _register.Modify(user!, new DomainUserRegisterContext() { SelectedRole = selectedRole});
            await _signInManager.RefreshSignInAsync(user);

            return LocalRedirect(Input.ReturnUrl ?? "/");
        }
    }


}
