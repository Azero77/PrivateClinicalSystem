using ClinicApp.Application.Common;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.QueryServices;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SecretaryAgg;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Common;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Infrastructure.QueryServices;
using ClinicApp.Infrastructure.Repositories;
using ClinicApp.Infrastructure.Services;
using ClinicApp.Shared.QueryTypes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<AppDbContext>((sp, opts) =>
        {
            opts.UseNpgsql(builder.Configuration.GetConnectionString("postgresClinicdb") ?? throw new ArgumentException("Connection string is null for clinic db"));
        });

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = builder.Configuration.GetConnectionString("cache") ?? throw new ArgumentException("Connection string is null for redis ");
            opts.InstanceName = "clinic-sessions";
        });

        services.AddScoped<ISessionRepository, DbSessionRepository>();
        services.AddScoped<DbDoctorRepository>();
        services.AddScoped<IDoctorRepository, CachedDbDoctorRepository>();
        services.AddScoped<DbRoomRepository>();
        services.AddScoped<IRoomRepository, CachedDbRoomRepository>();
        services.AddScoped<IPatientRepository, DbPatientRepository>();
        services.AddScoped<ISecretaryRepository, DbSecretaryRepository>();
        services.AddSingleton<IClock, Clock>();


        services.AddSingleton<IConverter<Doctor, DoctorDataModel>, DoctorConverter>();
        services.AddSingleton<IConverter<Room, RoomDataModel>, RoomConverter>();
        services.AddSingleton<IConverter<Session, SessionDataModel>, SessionConverter>();
        services.AddSingleton<IConverter<Patient, PatientDataModel>, PatientConverter>();
        services.AddSingleton<IConverter<Secretary, SecretaryDataModel>, SecretaryConverter>();
        services.AddScoped<IQueryService<SessionQueryType>, SessionQueryService>();
        services.AddScoped<IQueryService<DoctorQueryType>, DoctorQueryService>();
        services.AddScoped<IQueryService<RoomQueryType>, RoomQueryService>();
        services.AddScoped<IQueryService<SecretaryQueryType>, SecretaryQueryService>();
        services.AddScoped<IQueryService<PatientQueryType>, PatientQueryService>();
        services.AddScoped<IEventAdderService<SessionDomainEvent>, SessionEventAdderService>();
        //Mediatr is unable to register generic requestHandlers because DI with MSDI can support this kind of stuff
        RegisterMediatrGenericHandlers(services);
        return services;
    }
    private static void RegisterMediatrGenericHandler<T>(IServiceCollection services)
        where T:QueryType
    {
        services.AddScoped<IRequestHandler<QueryRequest<T>, IQueryable<T>>, QueryRequestHandler<T>>();
        services.AddScoped<IRequestHandler<QuerySingleRequest<T>, T?>, QuerySingleRequestHandler<T>>();

    }
    private static void RegisterMediatrGenericHandlers(IServiceCollection services)
    {
        RegisterMediatrGenericHandler<SessionQueryType>(services);
        RegisterMediatrGenericHandler<DoctorQueryType>(services);
        RegisterMediatrGenericHandler<PatientQueryType>(services);
        RegisterMediatrGenericHandler<RoomQueryType>(services);
        RegisterMediatrGenericHandler<SecretaryQueryType>(services);
    }

}