namespace ClinicApp.Infrastructure.Extensions;

using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.Seeding;
using ClinicApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureMiddlewareExtensions
{
    public static IApplicationBuilder  UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }

    internal static void SeedDataInDevelopment(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        {
            var provider = scope.ServiceProvider;
            using AppDbContext context = provider.GetRequiredService<AppDbContext>();

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
}

    