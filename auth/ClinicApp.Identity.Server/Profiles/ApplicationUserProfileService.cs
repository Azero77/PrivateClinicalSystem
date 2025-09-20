using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ClinicApp.Identity.Server.Profiles;

public class ApplicationUserProfileService : ProfileService<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

    public ApplicationUserProfileService(UserManager<ApplicationUser> userManager,
                                         IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory
                                         ) : base(userManager, claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    protected override Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {
        return base.GetProfileDataAsync(context, user);
    }
    protected override async Task IsActiveAsync(IsActiveContext context, ApplicationUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var claim = new Claim(ServerConstants.CompleteProfileClaimKey,ServerConstants.CompletedProfileClaimValue);
        var isActive = claims.FirstOrDefault(c => c.Type == claim.Type && c.Value == claim.Value) is not null;
        context.IsActive = isActive;
    }
}
