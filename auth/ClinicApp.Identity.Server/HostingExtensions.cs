using ClinicApp.Identity.Server.ConfigurationModels;
using ClinicApp.Identity.Server.Constants;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Pages;
using ClinicApp.Identity.Server.Profiles;
using ClinicApp.Identity.Server.Services;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Duende.IdentityModel;
using ClinicApp.Identity.Server.Services.Messaging;

namespace ClinicApp.Identity.Server;
internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();
        var isDev = builder.Environment.IsDevelopment();

        builder.Services.AddDbContext<UsersDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("no available connection string was found");
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddIdentity<ApplicationUser,ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.ExternalCookieAuthenticationScheme,
            opts =>
            {
                opts.Cookie.SameSite = SameSiteMode.None;
                opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        builder.Services.AddIdentityServer(options =>
        {
            options.IssuerUri = builder.Configuration?["Identity:Issuer"] ?? throw new ArgumentException("Issuer Was not provided");
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseSuccessEvents = true;
        })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ApplicationUserProfileService>()
            .AddLicenseSummary();
        var authentiation = builder.Services.AddAuthentication();
        AddExternalProviders(authentiation, builder);

        builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy(ServerConstants.RequireCompletedProfilePolicy, builder =>
            {
                builder.RequireClaim(ServerConstants.CompleteProfileClaimKey, ServerConstants.CompletedProfileClaimValue);
            });

            opts.AddPolicy(ServerConstants.UnCompletedProfilePolicy, builder =>
            {
                builder.RequireClaim(ServerConstants.CompleteProfileClaimKey);//no restriction on the claim value
            });
            opts.FallbackPolicy = opts.GetPolicy(ServerConstants.RequireCompletedProfilePolicy)!;
        });


        builder.Services.AddScoped<IEmailSender, LoggerEmailSender>();
        builder.Services.AddScoped<IDomainUserRegister, DomainUserRegister>();
        builder.Services.AddLoginFLow();
        builder.Services.AddMessaging();
        return builder.Build();
    }

    private static void AddExternalProviders(AuthenticationBuilder authentiation, WebApplicationBuilder builder)
    {
        var providers = builder.Configuration.GetSection("ExternalAuthProviders")
            .Get<IEnumerable<ExternalAuthProvider>>() ?? throw new ArgumentException("No external Providers have been provided");

        foreach (var provider in providers)
        {
            switch (provider.ProviderName)
            {
                case "Google":
                    authentiation.AddGoogle(opts =>
                    {
                        opts.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        opts.ClientId = provider.ClientId;
                        opts.ClientSecret = provider.ClientSecret;
                        opts.ClaimActions.Clear();
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.Subject, "sub");
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.Name, "name");
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.GivenName, "given_name");
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.FamilyName, "family_name");
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.Email, "email");
                        opts.ClaimActions.MapJsonKey(JwtClaimTypes.Picture, "picture");
                    });
                    break;
                default:
                    break;
            }
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages();
       
        return app;
    }
}
