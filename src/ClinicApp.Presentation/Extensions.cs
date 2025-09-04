using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.Seeding;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Presentation;

public static class DbExtensions
{
    /// <summary>
    /// Running Migrations in dev environment only
    /// </summary>
    /// <param name="app"></param>
    public static void RunMigrations(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            throw new SystemException("Can't apply migrations at runtime in prod");
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
        }
    }


    public static void SeedDataInDevelopment(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        {
            var provider = scope.ServiceProvider;
            using AppDbContext context = provider.GetRequiredService<AppDbContext>();
            SeedDataToDbContext(context);
        }

    }

    public static void SeedDataToDbContext(AppDbContext context)
    {

        if (!context.Rooms.Any())
        {
            context.Rooms.AddRange(SeedData.Rooms);
            context.SaveChanges();
        }

        // Seed Doctors
        if (!context.Doctors.Any())
        {
            context.Doctors.AddRange(SeedData.Doctors);
            context.SaveChanges();
        }

        // Seed Patients
        if (!context.Patients.Any())
        {
            context.Patients.AddRange(SeedData.Patients);
            context.SaveChanges();
        }

        // Seed Sessions
        if (!context.Sessions.Any())
        {
            foreach (var session in SeedData.Sessions)
            {
                // Assuming SeedData.Sessions is ErrorOr<Session>
                context.Sessions.Add(session);
            }
            context.SaveChanges();
        }

        // Seed OutBoxMessages
        if (!context.OutBoxMessages.Any())
        {
            context.OutBoxMessages.AddRange(SeedData.OutboxMessages);
            context.SaveChanges();
        }
    }
}
