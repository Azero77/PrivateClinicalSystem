using ClinicApp.Domain.Common;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Services;
using ClinicApp.Identity.Server.Services.Authentication;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace ClinicApp.Identity.Server.Pages.Account.CompleteRegistration
{
    //[Authorize(Policy = ServerConstants.UnCompletedProfilePolicy)]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserLoginService _userLoginService;
        public IEnumerable<SelectListItem> AllowedRoles => Enum.GetValues<UserRole>()
            .Where(DomainUserRegister.AllowedRoles.Contains)
            .Select(r => new SelectListItem()
            {
                Value = r.ToString(),
                Text = r.ToString()
            });

        public IndexModel(UserManager<ApplicationUser> userManager, UserLoginService userLoginService)
        {
            _userManager = userManager;
            _userLoginService = userLoginService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;
        public void OnGet()
        {

        }

        public IEnumerable<SelectListItem> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones()
                .Select(t => new SelectListItem()
                {
                    Text = t.DisplayName,
                    Value = t.Id
                });
        }
        public class InputModel
        {
            public string? ReturnUrl { get; set; }
            [Required]
            public UserRole Role { get; set; }

            [Required]
            public string? LastName { get; set; }

            [Required]
            public string? FirstName { get; set; }

            public DoctorInfo? DoctorInfo { get; set; }
        }

        public class DoctorInfo
        {
            public WorkingDays WorkingDays { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            [Required]
            public string? TimeZoneId { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            UserRole selectedRole = Input.Role;
            if (Input.DoctorInfo is not null && selectedRole != UserRole.Doctor)
            {
                ModelState.AddModelError("", "Invalid Data");
                return Page();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                ModelState.AddModelError("", "Unknown Error Happend,Try Again later");
                return Page();
            }
            string content = JsonSerializer.Serialize(
                new {
                    Input.ReturnUrl,
                    Input.Role,
                    Input.LastName,
                    Input.FirstName,
                    Input.DoctorInfo?.StartTime,
                    Input.DoctorInfo?.EndTime,
                    Input.DoctorInfo?.TimeZoneId,
                    Input.DoctorInfo?.WorkingDays,
                },new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull});
            var context = new DomainUserRegisterContext { SelectedRole = selectedRole,content = content };
            var result = await _userLoginService
                .CompleteRegistrationFor(user,
                Input?.ReturnUrl ?? "/",
                context);
            if (result.IsError)
            {
                ModelState.AddModelError("", "Unknown Error Happend,Try Again later");
                return Page();
            }
            return result.Value.ToActionResult(this);
        }
    }
}
