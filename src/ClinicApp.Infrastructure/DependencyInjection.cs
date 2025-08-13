using ClinicApp.Application.QueryServices;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.QueryServices;
using ClinicApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(connectionString);
            opts.LogTo(Console.WriteLine, LogLevel.Debug);
        });
        services.AddScoped<ISessionRepository, DbSessionRepository>();
        services.AddScoped<IDoctorRepository, DbDoctorRepository>();
        services.AddScoped<IRoomRepository, DbRoomRepository>();
        services.AddScoped<IDoctorQueryService, DoctorQueryService>();
        services.AddSingleton<IClock, Clock>();
        return services;
    }
}
