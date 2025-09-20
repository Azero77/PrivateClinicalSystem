using ClinicApp.Domain.Common;
using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Services;

public class DomainUserRegister : IDomainUserRegister
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public static UserRole[] AllowedRoles = { UserRole.Doctor, UserRole.Patient };
    public DomainUserRegister(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task Modify(ApplicationUser user,DomainUserRegisterContext context)
    {
        if (!AllowedRoles.Contains(context.SelectedRole))
        {
            throw new ArgumentException("Specified Role is not allowed");
        }
        await _userManager.AddToRoleAsync(user!, context.SelectedRole.ToString());
        var oldClaim = (await _userManager.GetClaimsAsync(user!)).FirstOrDefault(c => c.Type == ServerConstants.CompleteProfileClaimKey);
        if (oldClaim is not null)
        {
            var newClaim = new Claim(ServerConstants.CompleteProfileClaimKey, ServerConstants.CompletedProfileClaimValue);
            await _userManager.ReplaceClaimAsync(user!, oldClaim, newClaim);
        }
    }
}
