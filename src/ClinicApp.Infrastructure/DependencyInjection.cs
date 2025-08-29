using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionstring)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(connectionstring);
        });
        services.AddScoped<ISessionRepository, DbSessionRepository>();
        services.AddScoped<IDoctorRepository, DbDoctorRepository>();
        services.AddScoped<IRoomRepository, DbRoomRepository>();
        services.AddScoped<IPaginatedRepository<Session>, DbSessionRepository>();
        services.AddScoped<IPaginatedRepository<Doctor>, DbDoctorRepository>();
        services.AddScoped<IPaginatedRepository<Room>, DbRoomRepository>();
        services.AddSingleton<IClock, Clock>();
        return services;
    }
}
