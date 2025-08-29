using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;

namespace ClinicApp.Identity.Server;

public static class Seed
{
    public static async Task SeedDatabase(WebApplication app)
    {
        UsersDbContext context = app.Services.GetRequiredService<UsersDbContext>();
        UserManager<ApplicationUser> usrManager = app.Services.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager<ApplicationUser> roleManager = app.Services.GetRequiredService<RoleManager<ApplicationUser>>();


        //will add an admin user, secretary user, doctor user, patient user
        //roles will be added in OnModelCreating because it is part of the business logic and everything will break if no roles were added
    }
}
