using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace ClinicApp.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));
        return services;
    }
}
