using ClinicApp.Identity.Server.Infrastructure.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Versioning;
using System.Security.Policy;

namespace ClinicApp.Identity.Server.Services.Authentication.Middlwares;

public abstract class UserAuthenticationMiddleware
{
    protected UserAuthenticationMiddleware? _next = new TerminalUserAuthenticationMiddleware(); // default value if not set
    public UserAuthenticationMiddleware SetNext(UserAuthenticationMiddleware next)
    {
        _next = next;
        return next;
    }
    public abstract Task<IActionResult> Handle(ApplicationUser user, PageModel pageModel, string returnUrl);
}


public class CheckingOAuthContextUserAuthenticationMiddleware :
    UserAuthenticationMiddleware
{
    public override Task<IActionResult> Handle(ApplicationUser user, PageModel pageModel, string returnUrl)
    {
        throw new NotImplementedException();
    }
}