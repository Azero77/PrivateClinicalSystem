using ClinicApp.Application.Common;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.QueryServices;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Common;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Infrastructure.QueryServices;
using ClinicApp.Infrastructure.Repositories;
using ClinicApp.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionstring)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<AppDbContext>((sp, opts) =>
        {
            opts.UseNpgsql(connectionstring);
        });
        services.AddScoped<ISessionRepository, DbSessionRepository>();
        services.AddScoped<IDoctorRepository, DbDoctorRepository>();
        services.AddScoped<IRoomRepository, DbRoomRepository>();
        services.AddSingleton<IClock, Clock>();

        services.AddSingleton<IConverter<Doctor, DoctorDataModel>, DoctorConverter>();
        services.AddSingleton<IConverter<Room, RoomDataModel>, RoomConverter>();
        services.AddSingleton<IConverter<Session, SessionDataModel>, SessionConverter>();
        services.AddSingleton<IConverter<Patient, PatientDataModel>, PatientConverter>();
        services.AddScoped<IQueryService<SessionQueryType>, SessionQueryService>();
        services.AddScoped<IQueryService<DoctorQueryType>, DoctorQueryService>();
        services.AddScoped<IEventAdderService<SessionDomainEvent>, SessionEventAdderService>();
        //Mediatr is unable to register generic requestHandlers because DI with MSDI can support this kind of stuff
        RegisterMediatrGenericHandlers(services);
        return services;
    }

    private static void RegisterMediatrGenericHandlers(IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<QueryRequest<SessionQueryType>, IQueryable<SessionQueryType>>, QueryRequestHandler<SessionQueryType>>();
        services.AddScoped<IRequestHandler<QueryRequest<DoctorQueryType>, IQueryable<DoctorQueryType>>, QueryRequestHandler<DoctorQueryType>>();
        services.AddScoped<IRequestHandler<QuerySingleRequest<SessionQueryType>,SessionQueryType?>, QuerySingleRequestHandler<SessionQueryType>>();
        services.AddScoped<IRequestHandler<QuerySingleRequest<DoctorQueryType>,DoctorQueryType?>, QuerySingleRequestHandler<DoctorQueryType>>();
    }
}
