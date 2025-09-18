using ClinicApp.Identity.Server.Infrastructure.Persistance;
using ClinicApp.Identity.Server.Pages;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ClinicApp.Identity.Server;
internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();
        var isDev = builder.Environment.IsDevelopment();

        builder.Services.AddDbContext<UsersDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("no available connection string was found");
            options.UseNpgsql(connectionString);
        });
        builder.Services.AddIdentity<ApplicationUser,ApplicationRole>()
            .AddEntityFrameworkStores<UsersDbContext>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        builder.Services.AddIdentityServer(options =>
        {
            options.IssuerUri = builder.Configuration?["Identity:Issuer"] ?? throw new ArgumentException("Issuer Was not provided");
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.Authentication.CookieSameSiteMode = SameSiteMode.None;  
        })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddTestUsers(TestUsers.Users)
            .AddLicenseSummary();
        
        return builder.Build();
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
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
