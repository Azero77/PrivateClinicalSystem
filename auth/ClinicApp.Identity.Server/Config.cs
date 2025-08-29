using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ClinicApp.Identity.Server;
public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Address(),
            new IdentityResources.Phone()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { new ApiScope("api","Management API")};

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        string bff_url = configuration?["Identity__Bff_url"] ?? throw new ArgumentException("No Bff Url provided");
        string secret = configuration?["Identity__Secret"] ?? throw new ArgumentException("No Secret is provided");
        return new Client[]
            {
                new Client()
                {
                    ClientId = "bff",
                    ClientSecrets = [new Secret(secret)],
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { $"{bff_url}/signin-oidc"},
                    FrontChannelLogoutUri = $"{bff_url}/signout-oidc",
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api",
                    },
                    AllowOfflineAccess = true,
                }
            };
    }
}
