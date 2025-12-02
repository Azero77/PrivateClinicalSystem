using ClinicApp.Domain.Services.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Domain;
public static class DependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IScheduler, DoctorScheduler>();

        return services;
    }
}
