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
            new IdentityResources.Phone(),
            new IdentityResource(JwtClaimTypes.Role, new string[] { "role" })
        };
        
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { new ApiScope("api","Management API")};

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        { new ApiResource("api")
            {
                Scopes = {"api" },
                UserClaims = { JwtClaimTypes.Role }
            }
        };

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        string[] bff_urls = configuration.GetSection("Identity:Bff_Urls").Get<string[]>() ?? throw new ArgumentException("No Bff Url provided");
        List<string> getUrlsWithSuffix(string[] urls,string suffix)
        {
            List<string> inner = new();
            foreach (var url in urls)
            {
                inner.Add($"{url}{suffix}");
            }
            return inner;
        }
        string secret = configuration?["Identity:Secret"] ?? throw new ArgumentException("No Secret is provided");
        return new Client[]
            {
                new Client()
                {
                    ClientId = "bff",
                    ClientSecrets = {new Secret(secret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris =  getUrlsWithSuffix(bff_urls, "/signin-oidc"),
                    PostLogoutRedirectUris =  getUrlsWithSuffix(bff_urls,"signout-callback-oidc"),
                    AllowedCorsOrigins = bff_urls,
                    FrontChannelLogoutUri = $"{bff_urls[0]}/signout-oidc",
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        JwtClaimTypes.Role,
                        "api",
                    },
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
    }
}
