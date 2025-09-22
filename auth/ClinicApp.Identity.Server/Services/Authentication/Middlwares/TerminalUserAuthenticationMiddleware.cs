using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public class TerminalUserAuthenticationMiddleware : UserAuthenticationMiddleware
{
    public override async Task<IActionResult> Handle(ApplicationUser user, PageModel pageModel, string returnUrl)
    {
        return pageModel.LocalRedirect(returnUrl);
    }
}
