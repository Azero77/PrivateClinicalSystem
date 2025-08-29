using ClinicApp.Domain.Common;
using ClinicApp.Identity.Server.Infrastructure.Persistance;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicApp.Identity.Server;

public static class Seed
{
    public static async Task SeedDatabase(WebApplication app)
    {
        // It's good practice to scope the services here
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            // The RoleManager manages 'ApplicationRole', not 'ApplicationUser'
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            // Ensure the database is created and migrations are applied
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed users only if none exist
            if (!await context.Users.AnyAsync())
            {
                // 1. Admin User
                var adminUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin@clinic.com",
                    Email = "admin@clinic.com",
                    EmailConfirmed = true,
                    PhoneNumber = "111-222-3333"
                };
                var adminResult = await userManager.CreateAsync(adminUser, "AdminPass123$");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
                    await userManager.AddClaimsAsync(adminUser, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Admin User"),
                        new Claim(JwtClaimTypes.GivenName, "Admin"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                    });
                }

                // 2. Doctor User
                var doctorUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "doctor@clinic.com",
                    Email = "doctor@clinic.com",
                    EmailConfirmed = true,
                    PhoneNumber = "444-555-6666"
                };
                var doctorResult = await userManager.CreateAsync(doctorUser, "DoctorPass123$");
                if (doctorResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(doctorUser, UserRole.Doctor.ToString());
                    await userManager.AddClaimsAsync(doctorUser, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Wonderland"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Wonderland"),
                    });
                }

                // 3. Secretary User
                var secretaryUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "secretary@clinic.com",
                    Email = "secretary@clinic.com",
                    EmailConfirmed = true,
                    PhoneNumber = "777-888-9999"
                };
                var secretaryResult = await userManager.CreateAsync(secretaryUser, "SecretaryPass123$");
                if (secretaryResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(secretaryUser, UserRole.Secretary.ToString());
                    await userManager.AddClaimsAsync(secretaryUser, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Builder"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Builder"),
                    });
                }

                // 4. Patient User
                var patientUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "patient@clinic.com",
                    Email = "patient@clinic.com",
                    EmailConfirmed = true,
                    PhoneNumber = "123-456-7890"
                };
                var patientResult = await userManager.CreateAsync(patientUser, "PatientPass123$");
                if (patientResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(patientUser, UserRole.Patient.ToString());
                    await userManager.AddClaimsAsync(patientUser, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Charlie Brown"),
                        new Claim(JwtClaimTypes.GivenName, "Charlie"),
                        new Claim(JwtClaimTypes.FamilyName, "Brown"),
                    });
                }
            }
        }
    }
}