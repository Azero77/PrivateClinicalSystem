using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure;
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


    
}
