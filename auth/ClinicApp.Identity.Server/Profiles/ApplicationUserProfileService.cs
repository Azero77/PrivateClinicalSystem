using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Duende.IdentityServer.AspNetIdentity;
using Microsoft.AspNetCore.Identity;

namespace ClinicApp.Identity.Server.Profiles;

public class ApplicationUserProfileService : ProfileService<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

    public ApplicationUserProfileService(UserManager<ApplicationUser> userManager,
                                         IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
                                         ) : base(userManager, claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }
}
