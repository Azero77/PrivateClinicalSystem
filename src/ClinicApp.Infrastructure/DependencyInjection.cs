using ClinicApp.Application;
using ClinicApp.Application.Common;
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
        services.AddProxidScoped<ISessionRepository, DbSessionRepository, AggregateTrackerInterceptor>();
        services.AddProxidScoped<IDoctorRepository, DbDoctorRepository,AggregateTrackerInterceptor>();
        services.AddProxidScoped<IRoomRepository, DbRoomRepository,AggregateTrackerInterceptor>();


        services.AddScoped<IPaginatedRepository<Session>, DbSessionRepository>();
        services.AddScoped<IPaginatedRepository<Doctor>, DbDoctorRepository>();
        services.AddScoped<IPaginatedRepository<Room>, DbRoomRepository>();
        services.AddScoped<IUnitOfWork,UnitOfWork.UnitOfWork>();
        services.AddSingleton<IClock, Clock>();
        return services;
    }
}
