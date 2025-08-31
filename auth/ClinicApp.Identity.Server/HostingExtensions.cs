using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace ClinicApp.Identity.Server;
internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        
        // uncomment if you want to add a UI
        //builder.Services.AddRazorPages();
        var isDev = builder.Environment.IsDevelopment();

        builder.Services.AddDbContext<UsersDbContext>(options =>
        {
            string connectionString = builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("no available connection string was found");
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddIdentity<ApplicationUser,ApplicationRole>()
            .AddEntityFrameworkStores<UsersDbContext>();
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
            .AddInMemoryClients(Config.Clients(builder.Configuration))
            .AddAspNetIdentity<ApplicationUser>()
            .AddSigningCredential(GetKey(builder),SecurityAlgorithms.HmacSha256) //temp for dev
            .AddLicenseSummary();
        
        return builder.Build();
    }

    private static SymmetricSecurityKey GetKey(WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment())
            throw new ArgumentException("Production Can't have symmetric keys");
        // This secret must be the base64 encoded key from 'dotnet user-jwts'
        string secretKey = builder.Configuration?["Identity:Key"] ?? throw new ArgumentException("No Secret Key Was Provided");

        var bytes = Convert.FromBase64String(secretKey);

        return new SymmetricSecurityKey(bytes);

    }
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        //app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        //app.UseAuthorization();
        //app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
