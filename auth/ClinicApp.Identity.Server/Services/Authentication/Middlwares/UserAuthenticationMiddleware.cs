using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ErrorOr;
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
    public abstract Task<ErrorOr<LoginResult>> Handle(ApplicationUser user, string returnUrl);
}
