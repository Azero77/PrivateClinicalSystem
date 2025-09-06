namespace ClinicApp.Infrastructure.Extensions;

using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Infrastructure.Middlewares;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureMiddlewareExtensions
{
    public static IApplicationBuilder  UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }

    public static void SeedDataInDevelopment(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        {
            var provider = scope.ServiceProvider;
            IClock clock = provider.GetRequiredService<IClock>();
            using AppDbContext context = provider.GetRequiredService<AppDbContext>();
            SeedDataToDbContext(context, clock);
        }

    }

    public static void SeedDataToDbContext(AppDbContext context, IClock? clock = null)
    {
        if (clock is null)
            clock = new Clock();
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
            foreach (var session in SeedData.Sessions(clock))
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

    